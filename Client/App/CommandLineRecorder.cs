using SysDVR.Client.Core;
using SysDVR.Client.Targets.FileOutput;
using System;
using System.Threading;

namespace SysDVR.Client.App
{
    // The text of fully-command line execution modes is not localized
    internal class CommandLineRecorder(CommandLineOptions CommandLine) : BaseStandaloneClient
    {
        readonly CommandLineOptions CommandLine = CommandLine;
        bool printErrors = true;

        void ErrorHandler(string msg)
        {
            if (printErrors)
                Console.WriteLine(msg);
        }

        StreamManager? ConnectToConsole(Mp4Output recorder)
        {
            var conn = GetConnector(CommandLine);

            if (conn is not null)
            {
                conn.OnMessage += ErrorHandler;
                var device = conn.ConnectWithTargets(recorder.VideoTarget, recorder.AudioTarget).GetAwaiter().GetResult();

                device.OnErrorMessage += ErrorHandler;
                return device;
            }

            return null;
        }

        public override void Entrypoint()
        {
            bool cancel = false;

            var filename = CommandLine.OutputFile ?? throw new InvalidOperationException("No output file specified");
            var hasVideo = Program.Options.Streaming.HasVideo;
            var hasAudio = Program.Options.Streaming.HasAudio;

            using var output = new Mp4Output(filename, 
                hasVideo ? new() : null,
                hasAudio ? new() : null);

            output.Start();

            Console.WriteLine($"Attempting to connect...");
            using var console = ConnectToConsole(output);
            console.OnFatalError += ex =>
            {
                Console.WriteLine("Fatal error occurred: " + ex.Message);
                cancel = true;
            };

            console.Begin();

            Console.WriteLine("Recording started. Press Return or CTRL+C to stop.");
            Console.WriteLine("Do NOT terminate the process, that might corrupt the output file.");

            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                Console.WriteLine("CTRL+C Received, exiting...");
                cancel = true;
            };

            while (!cancel)
            {
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Enter)
                {
                    Console.WriteLine("Return key pressed, exiting...");
                    break;
                }
                Thread.Sleep(100);
            }

            // We're disposing things, don't print errors about disconnection
            printErrors = false;

            output.Stop();
        }

        public override void Initialize()
        {
            Console.WriteLine("Starting command line recorder...");
        }
    }
}
