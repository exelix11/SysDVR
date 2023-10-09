#if DEBUG
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using SysDVR.Client.Core;

namespace SysDVR.Client.Targets.FileOutput
{
    class LoggingTarget : OutStream
    {
        readonly BinaryWriter bin;
        readonly MemoryStream mem = new MemoryStream();
        readonly string filename;

        public LoggingTarget(string filename)
        {
            this.filename = filename;
            bin = new BinaryWriter(mem);
        }

        public void FlushToDisk()
        {
            File.WriteAllBytes(filename, mem.ToArray());
        }

        Stopwatch sw = new Stopwatch();

        protected override void SendDataImpl(PoolBuffer data, ulong ts)
        {
            Console.WriteLine($"{filename} - ts: {ts}");
            bin.Write(0xAAAAAAAA);
            bin.Write(sw.ElapsedMilliseconds);
            bin.Write(ts);
            bin.Write(data.Length);
            bin.Write(data.Span);
            sw.Restart();

            data.Free();
        }

        protected override void DisposeImpl()
        {
            mem.Dispose();
            bin.Dispose();
            base.Dispose();
        }
    }

    class LoggingManager : BaseStreamManager
    {
        public LoggingManager(string VPath, string APath, CancellationTokenSource cancel) : base(
            VPath != null ? new LoggingTarget(VPath) : null,
            APath != null ? new LoggingTarget(APath) : null,
            cancel)
        {

        }

        public override void Stop()
        {
            base.Stop();
            (VideoTarget as LoggingTarget)?.FlushToDisk();
            (AudioTarget as LoggingTarget)?.FlushToDisk();
        }
    }
}
#endif
