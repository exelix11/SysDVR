//#define EXCEPTION_DEBUG

using SysDVR.Client.Sources;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace SysDVR.Client.Core
{
    abstract class StreamThread : IDisposable
    {
        Thread DeviceThread;
        CancellationTokenSource Cancel;

        readonly BaseStreamManager Manager;
        readonly public StreamingSource Source;
        readonly public StreamKind Kind;

        protected abstract void SetCancellationToken(CancellationToken token);
        protected abstract bool DataReceived(in PacketHeader header, PoolBuffer body);

#if MEASURE_STATS
		public ulong ReceivedBytes { get; private set; }
		public DateTime FirstByteTs { get; private set; }

		public void StatsReceivedData(int length)
        {
			if (ReceivedBytes == 0)
				FirstByteTs = DateTime.Now;
			ReceivedBytes += (uint)length + PacketHeader.StructLength;
        }
#endif

        protected StreamThread(StreamingSource source, BaseStreamManager manager)
        {
            Source = source;
            Kind = Source.SourceKind;
            Manager = manager;

            source.OnMessage += Manager.ReportError;
        }

        public void Start()
        {
            Cancel = new CancellationTokenSource();

            DeviceThread = new Thread(() => DeviceThreadMain(Cancel.Token));
            DeviceThread.Name = "DeviceThread for " + Kind;

            DeviceThread.Start();
        }

        public void Stop()
        {
            Cancel.Cancel();
            Source.StopStreaming();
        }

        public void Join()
        {
            DeviceThread.Join();
        }

        TimeTrace trace = new TimeTrace();
        void DeviceThreadMain(CancellationToken token)
        {
#if ANDROID_LIB
            // This thread may try to reopen USB devices in case of errors
            Program.Native.NativeAttachThread?.Invoke();
#endif
            var logStats = DebugOptions.Current.Stats;
            var logDbg = DebugOptions.Current.Log;

            SetCancellationToken(token);

            var HeaderData = new byte[PacketHeader.StructLength];
            ref var Header = ref MemoryMarshal.Cast<byte, PacketHeader>(HeaderData)[0];
            try
            {
            loop_again:
                while (!token.IsCancellationRequested)
                {
                    while (!Source.ReadHeader(HeaderData))
                    {
                        Source.Flush();
                        goto loop_again;
                    }

                    if (logStats)
                        Manager.ReportError($"[{Kind}] {Header}");

                    if (Header.Magic != PacketHeader.MagicResponse)
                    {
                        if (logDbg)
                            Manager.ReportError($"[{Kind}] Wrong header magic: {Header.Magic:X}");

                        Source.Flush();
                        continue;
                    }

                    if (Header.DataSize > StreamInfo.MaxPayloadSize)
                    {
                        if (logDbg)
                            Manager.ReportError($"[{Kind}] Data size exceeds max size: {Header.DataSize:X}");

                        Source.Flush();
                        continue;
                    }

                    if (Header.IsHash)
                    {
                        // TODO....
                    }

                    var Data = PoolBuffer.Rent(Header.DataSize);
                    if (!Source.ReadPayload(Data.RawBuffer, Header.DataSize))
                    {
                        if (logDbg)
                            Manager.ReportError($"[{Kind}] Read payload failed.");

                        Source.Flush();
                        Data.Free();
                        continue;
                    }

                    if (!DataReceived(Header, Data))
                    {
                        if (logDbg)
                            Manager.ReportError($"[{Kind}] DataReceived rejected the packet, header magic was {Header.Magic:X}");

                        Data.Free();
                        continue;
                    }
                }

            }
#if !EXCEPTION_DEBUG || RELEASE
            catch (Exception ex)
            {
                if (!token.IsCancellationRequested)
                    Manager.ReportFatalError(ex);
            }
#endif

#if ANDROID_LIB
            Program.Native.NativeDetachThread?.Invoke();
#endif
        }

        public void Dispose()
        {
            if (DeviceThread.IsAlive)
                throw new Exception($"{Kind} Thread is still running");

            Cancel.Dispose();
        }
    }

    class SingleStreamThread : StreamThread
    {
        readonly OutStream Target;

        public SingleStreamThread(StreamingSource source, OutStream target, BaseStreamManager manager) : base(source, manager)
        {
            Target = target;
        }

        protected override void SetCancellationToken(CancellationToken token)
        {
            Target.UseCancellationToken(token);
        }

        protected override bool DataReceived(in PacketHeader header, PoolBuffer body)
        {
            var valid =
                Kind == StreamKind.Video && header.IsVideo ||
                Kind == StreamKind.Audio && header.IsAudio;

            if (!valid)
                return false;

            Target.SendData(body, header.Timestamp);

            return true;
        }
    }

    class MultiStreamThread : StreamThread
    {
        readonly OutStream VideoTarget;
        readonly OutStream AudioTarget;

        public MultiStreamThread(StreamingSource source, OutStream videoTarget, OutStream audioTarget, BaseStreamManager manager) : base(source, manager)
        {
            if (source.SourceKind != StreamKind.Both)
                throw new Exception("Source must be able to provide both streams");

            VideoTarget = videoTarget;
            AudioTarget = audioTarget;
        }

        protected override void SetCancellationToken(CancellationToken token)
        {
            VideoTarget.UseCancellationToken(token);
            AudioTarget.UseCancellationToken(token);
        }

        protected override bool DataReceived(in PacketHeader header, PoolBuffer body)
        {
            if (header.IsVideo)
                VideoTarget.SendData(body, header.Timestamp);
            else if (header.IsAudio)
                AudioTarget.SendData(body, header.Timestamp);
            else
                return false;

            return true;
        }
    }
}
