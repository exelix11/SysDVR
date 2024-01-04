using SysDVR.Client.Sources;
using SysDVR.Client.Targets;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Channels;
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
		public bool UseNALReplayOnlyOnKeyframes = false;

		public bool HasVideo => Kind is StreamKind.Video or StreamKind.Both;
		public bool HasAudio => Kind is StreamKind.Audio or StreamKind.Both;

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
				UseNALReplayOnlyOnKeyframes = UseNALReplayOnlyOnKeyframes
			};
		}
	}

	class PoolBuffer
	{
		private readonly static ArrayPool<byte> pool = ArrayPool<byte>.Shared;

		public int Length { get; private set; }
		private byte[] _buffer;
		private int refcount;

		public byte[] RawBuffer => _buffer ?? throw new Exception("The buffer has been freed");

		public void Reference()
		{
			Interlocked.Increment(ref refcount);
		}

		public void Free()
		{
			Interlocked.Decrement(ref refcount);

#if DEBUG
			if (refcount < 0)
				throw new Exception("Buffer refcount is negative");
#endif

			if (refcount == 0)
			{
				pool.Return(RawBuffer);
				_buffer = null;
				GC.SuppressFinalize(this);
			}
		}

		public static PoolBuffer Rent(int len)
		{
			if (len == 0)
				throw new Exception("Invalid lngth");
			return new PoolBuffer(pool.Rent(len), len);
		}

		private PoolBuffer(byte[] buf, int len)
		{
			Length = len;
			_buffer = buf;
			refcount = 1;
		}

		~PoolBuffer()
		{
			if (refcount != 0)
			{
#if DEBUG
				Console.WriteLine($"Buffer was not freed {refcount}");
#endif
				refcount = 1;
				Free();
			}
		}

		public Span<byte> Span =>
			new Span<byte>(RawBuffer, 0, Length);

		public Memory<byte> Memory =>
			new Memory<byte>(RawBuffer, 0, Length);

		public ArraySegment<byte> ArraySegment =>
			new ArraySegment<byte>(RawBuffer, 0, Length);

		public static implicit operator Span<byte>(PoolBuffer o) => o.Span;
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

		protected virtual void UseCancellationTokenImpl(CancellationToken tok)
		{
			Cancel = tok;
			Next?.UseCancellationToken(tok);
		}

		protected abstract void SendDataImpl(PoolBuffer block, ulong ts);

		// This must be called before sending any data
		public void UseCancellationToken(CancellationToken tok)
		{
			UseCancellationTokenImpl(tok);
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

					if (packet.Header.IsAudio)
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
									Console.WriteLine("Unknown hash value, skipping packet");
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
				packet.Buffer?.Free();
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
