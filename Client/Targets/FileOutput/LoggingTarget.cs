#if DEBUG
using System;
using System.Diagnostics;
using System.Formats.Asn1;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SysDVR.Client.Core;
using SysDVR.Client.Sources;

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
        public LoggingManager(StreamingSource source, string VPath, string APath, CancellationTokenSource cancel) : base(
            source,
            VPath != null ? new LoggingTarget(VPath) : null,
            APath != null ? new LoggingTarget(APath) : null,
            cancel)
        {

        }

        public override async Task Stop()
        {
            await base.Stop().ConfigureAwait(false);
            (VideoTarget as LoggingTarget)?.FlushToDisk();
            (AudioTarget as LoggingTarget)?.FlushToDisk();
        }
    }
}
#endif
