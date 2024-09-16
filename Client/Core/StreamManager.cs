using SysDVR.Client.Sources;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SysDVR.Client.Core
{
	public enum StreamKind
	{
		Both,
		Video,
		Audio
	};

	public class StreamingOptions
	{
		public StreamKind Kind = StreamKind.Both;
		public int AudioBatching = 3;
		public bool UseNALReplay = true;
		public bool UseNALReplayOnlyOnKeyframes = true;

		public bool HasVideo => Kind is StreamKind.Video or StreamKind.Both;
		public bool HasAudio => Kind is StreamKind.Audio or StreamKind.Both;

		public bool TurnOffConsoleScreen = false;

		public bool Validate()
		{
			if (AudioBatching < 0 || AudioBatching > StreamInfo.MaxAudioBatching)
				return false;

			return true;
		}

		public StreamingOptions Clone()
		{
			return new StreamingOptions
			{
				Kind = Kind,
				AudioBatching = AudioBatching,
				UseNALReplay = UseNALReplay,
				UseNALReplayOnlyOnKeyframes = UseNALReplayOnlyOnKeyframes,
				TurnOffConsoleScreen = TurnOffConsoleScreen
			};
		}
	}

	class PoolBuffer
	{
		readonly static ArrayPool<byte> bufferPool = ArrayPool<byte>.Shared;
		readonly static ConcurrentBag<PoolBuffer> instancePool = new();

		public int Length { get; private set; }
		private byte[] buffer;
		private int refcount;

		public bool IsFree => refcount <= 0;
		
		public byte[] GetRawArray([CallerMemberName] string? caller = null) => buffer ?? throw new Exception($"The buffer has been freed [{caller}]");

		private static PoolBuffer GetInstance() 
		{
			if (instancePool.TryTake(out var instance))
				return instance;

			return new PoolBuffer();
		}

		private static void ReturnToPool(PoolBuffer buffer) 
		{
			if (!buffer.IsFree)
				throw new Exception("Attempted to return a non-free buffer to the pool");

			buffer.Length = 0;
			buffer.refcount = 0;
			buffer.buffer = null;

			instancePool.Add(buffer);
		}

		private void Configure(byte[] buf, int len)
		{
			if (buf == null)
				throw new Exception("Can't initialize a pooled buffer from null");

			Length = len;
			buffer = buf;
			refcount = 1;
		}

		public void Reference()
		{
			if (IsFree)
				throw new Exception("Attempted to reference an invalid buffer");

			Interlocked.Increment(ref refcount);
		}

		// It is actually fine to not call free and leak this buffer,
		// The GC will collect it but next time we need a buffer it will be allocated from scratch adding GC pressure and potentially stuttering
		public void Free()
		{
			if (IsFree)
				throw new Exception("Attempted to free an invalid buffer");

			var curCount = Interlocked.Decrement(ref refcount);

			if (curCount < 0)
				throw new Exception("Buffer refcount is negative");

			if (curCount == 0)
			{
				bufferPool.Return(buffer);
				ReturnToPool(this);
			}
		}

		public static PoolBuffer Rent(int len)
		{
			if (len == 0)
				throw new Exception("Invalid lngth");
			
			var res = GetInstance();
			res.Configure(bufferPool.Rent(len), len);
			return res;
		}

		public Span<byte> Span =>
			new Span<byte>(GetRawArray(), 0, Length);

		public Memory<byte> Memory =>
			new Memory<byte>(GetRawArray(), 0, Length);

		public ArraySegment<byte> ArraySegment =>
			new ArraySegment<byte>(GetRawArray(), 0, Length);
	}

	abstract class OutStream : IDisposable
	{
		protected OutStream? Next;
		protected CancellationToken Cancel;
		bool _disposed;

		public void ChainStream(OutStream toAdd)
		{
			if (Next is not null)
				Next.ChainStream(toAdd);
			else
				Next = toAdd;
		}

		// The caller must keep a reference to the stream as it must be manually disposed
		public bool UnchainStream(OutStream toRemove)
		{
			if (Next == toRemove)
			{
				var n = Next;

				Next = n.Next;
				n.Next = null;

				return true;
			}
			else if (Next is not null)
				return Next.UnchainStream(toRemove);
			else
				return false;
		}

		protected abstract void SendDataImpl(PoolBuffer block, ulong ts);

		// This must be called before sending any data
		public virtual void UseCancellationToken(CancellationToken tok)
		{
			Cancel = tok;
			Next?.UseCancellationToken(tok);
		}

		public void SendData(PoolBuffer block, ulong ts)
		{
			// Reference counting:
			//      block starts with refcount = 1
			//      SendData() increases it before calling the impl
			//      Each impl calls Free() when it doesn't need it anymore (even asynchronously)
			//      The final block in the chain calls Free() to remove the initial refcount, any pending refs are cleared by async processes
			//      When ref == 0 the block is freed
			block.Reference();
			SendDataImpl(block, ts);

			if (Next is not null)
				Next.SendData(block, ts);
			else
				block.Free();
		}

		protected virtual void DisposeImpl()
		{

		}

		public void Dispose()
		{
			if (_disposed)
				return;

			_disposed = true;

			DisposeImpl();
			Next?.Dispose();
		}
	}

	abstract class BaseStreamManager : IDisposable
	{
		private bool disposedValue;

		public event Action<Exception>? OnFatalError;
		public event Action<string>? OnErrorMessage;

		private Task? StreamingTask;

		protected OutStream? VideoTarget { get; set; }
		protected OutStream? AudioTarget { get; set; }
		protected StreamingSource Source { get; set; }
		protected StreamingOptions Options => Source.Options;

		public bool HasVideo => Options.HasVideo;
		public bool HasAudio => Options.HasAudio;

		// This is only used for video streams when NAL hashes are enabled
		readonly PacketReplayTable Replay = new();

		readonly CancellationTokenSource Cancel;

		public void ReportError(string message) =>
			OnErrorMessage?.Invoke(message);

		public void ReportFatalError(Exception ex) =>
			OnFatalError?.Invoke(ex);

		public BaseStreamManager(StreamingSource source, OutStream? videoTarget, OutStream? audioTarget, CancellationTokenSource cancel)
		{
			Source = source;
			VideoTarget = videoTarget;
			AudioTarget = audioTarget;
			Cancel = cancel;
		}

		public void ChainTargets(OutStream? nextVideo, OutStream? nextAudio)
		{
			if (nextVideo is not null)
				VideoTarget?.ChainStream(nextVideo);

			if (nextAudio is not null)
				AudioTarget?.ChainStream(nextAudio);
		}

		public void UnchainTargets(OutStream? nextVideo, OutStream? nextAudio)
		{
			if (nextVideo is not null)
				VideoTarget?.UnchainStream(nextVideo).AssertTrue("The target was not in the streaming chain");

			if (nextAudio is not null)
				AudioTarget?.UnchainStream(nextAudio).AssertTrue("The target was not in the streaming chain");
		}

		public virtual void Begin()
		{
			// Sanity checks
			if (Source is null)
				throw new Exception("No streams have been set");

			if (StreamingTask is not null)
				throw new Exception("The streaming has already started");

			if (Options.HasAudio && AudioTarget is null)
				throw new Exception("The audio target is missing");
			if (Options.HasVideo && VideoTarget is null)
				throw new Exception("The video target is missing");

			if (Program.Options.Debug.RequiresH264Analysis && VideoTarget is not null)
				VideoTarget.ChainStream(new H264LoggingTarget());

			StreamingTask = StreamTask();
		}

		async Task StreamTask()
		{
			ReceivedPacket packet = default;
			try
			{
				var useHash = Options.UseNALReplay;
				var token = Cancel.Token;

				VideoTarget?.UseCancellationToken(token);
				AudioTarget?.UseCancellationToken(token);

				while (!token.IsCancellationRequested)
				{
					try
					{
						packet = await Source.ReadNextPacket().ConfigureAwait(false);
					}
					catch (Exception ex)
					{
						OnErrorMessage?.Invoke($"Error reading next packet: {ex.Message}");
						await Source.Flush().ConfigureAwait(false);
						continue;
					}

					// This flag only exists starting with protocol version 03
					if (packet.Header.IsError)
					{
						var error = PacketErrorParser.GetPacketErrorAsString(packet);
						
						if (Program.Options.Debug.ConsoleErrors)
							OnErrorMessage?.Invoke(PacketErrorParser.GetPacketErrorAsString(packet));
						else
							Console.WriteLine(error);

						packet.Buffer?.Free();
					}
					else if (packet.Header.IsAudio)
						AudioTarget.SendData(packet.Buffer, packet.Header.Timestamp);
					else
					{
						if (useHash)
						{
							if (packet.Header.IsReplay)
							{
								packet.Buffer?.Free();
								if (!Replay.LookupSlot(packet.Header.ReplaySlot, out packet.Buffer))
								{
									ReportError("Unknown hash value, skipping packet");
									continue;
								}
							}
							else if (packet.Header.ReplaySlot != 0xFF)
							{
								Replay.AssignSlot(packet.Buffer, packet.Header.ReplaySlot);
							}
						}

						VideoTarget.SendData(packet.Buffer, packet.Header.Timestamp);
					}

					packet.Buffer = null;
				}
			}
			catch (OperationCanceledException) { }
			catch (Exception ex)
			{
				ReportFatalError(ex);
			}
			finally
			{
				if (packet.Buffer is { IsFree: false })
					packet.Buffer.Free();
			}
		}

		public virtual async Task Stop()
		{
			if (StreamingTask is null)
				return;

			Cancel.Cancel();
			try
			{
				await StreamingTask.ConfigureAwait(false);
			}
			catch (OperationCanceledException)
			{
				// Ignore
			}
			StreamingTask = null;
#if MEASURE_STATS
			var vDiff = DateTime.Now - VideoThread.FirstByteTs;
			var aDiff = DateTime.Now - AudioThread.FirstByteTs;
			var total = VideoThread.ReceivedBytes + AudioThread.ReceivedBytes;

			var max = vDiff > aDiff ? vDiff : aDiff;

			Console.WriteLine($"MEASURE_STATS: received {total} bytes in {max.TotalSeconds} s of streaming, avg of {total / max.TotalSeconds} B/s.");
			Console.WriteLine($"Per thread stats:");
			Console.WriteLine($"\tVideo: {VideoThread.ReceivedBytes} bytes in {vDiff.TotalSeconds} s, avg of {VideoThread.ReceivedBytes / vDiff.TotalSeconds} B/s");
			Console.WriteLine($"\tAudio: {AudioThread.ReceivedBytes} bytes in {aDiff.TotalSeconds} s, avg of {AudioThread.ReceivedBytes / aDiff.TotalSeconds} B/s");
#endif
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					Stop().GetAwaiter().GetResult();

					if (Source is IDisposable s)
						s.Dispose();
					if (VideoTarget is IDisposable iv)
						iv.Dispose();
					if (AudioTarget is IDisposable ia)
						ia.Dispose();

					Replay?.Dispose();
				}

				disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
