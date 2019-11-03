using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace UsbStream
{
	class PipedOutTarget_Win : IOutTarget
	{
		NamedPipeServerStream pipe;
		string PipeName;

		public event IOutTarget.ClientConnectedDelegate ClientConnected;

		public PipedOutTarget_Win(string pipeName)
		{
			PipeName = pipeName;
		}

		public void Dispose()
		{
			pipe.Close();
			pipe.Dispose();
		}

		public void SendData(byte[] data, int offset, int size)
		{
			if (!pipe.IsConnected) Ready();
			try
			{
				pipe.Write(data, offset, size);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"WARNING - pipe {PipeName} : {ex.Message}");
				Ready();
			}
		}

		public void Ready()
		{
			pipe?.Dispose();
			pipe = new NamedPipeServerStream(PipeName, PipeDirection.Out, 1, PipeTransmissionMode.Byte);
			Console.WriteLine($"Waiting for connection on pipe {PipeName}");
			pipe.WaitForConnection();
			ClientConnected();
		}
	}

	class PipedOutTarget_Linux : IOutTarget
	{
		FileStream pipe;
		string PipeName;

		public event IOutTarget.ClientConnectedDelegate ClientConnected;

		public PipedOutTarget_Linux(string pipeName)
		{
			PipeName = "/tmp/" + pipeName;
			Mono.Unix.Native.Syscall.mkfifo(pipeName, Mono.Unix.Native.FilePermissions.ACCESSPERMS);
			pipe = File.Open(PipeName, FileMode.Open, FileAccess.Write);
		}

		public void Dispose()
		{
			pipe.Close();
			pipe.Dispose();
			Mono.Unix.Native.Syscall.unlink(PipeName);
		}

		public void SendData(byte[] data, int offset, int size)
		{
			pipe.Write(data, offset, size);
		}

		public void Ready() { ClientConnected(); }
	}

	static class LiveStreamHelper
	{
		const string VPipeName = "UsbStreamV";
		const string APipeName = "UsbStreamA";

		static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

		//TODO test linux support
		static string VPipeFile => IsWindows ? $"//./pipe/{VPipeName}" : $"/tmp/{VPipeName}";
		static string APipeFile => IsWindows ? $"//./pipe/{APipeName}" : $"/tmp/{APipeName}";

		static string FFmpegPath = "";
		static string FFmpegArgs = "";

		public static (IOutTarget, IOutTarget) ParseArgs(string[] args)
		{
			IOutTarget VTarget = null, ATarget = null;

			FFmpegPath = args[1];
			string FFmpegOut = args[2];

			//Todo: more tests with -vcodec copy
			FFmpegArgs = $"-fflags +genpts -f s16le -ar 48000 -ac 2 -i \"{APipeFile}\" -f h264 -r 30 -framerate 30 -i \"{VPipeFile}\" -vcodec libx264 -pix_fmt yuv420p -b:v 2500k -x264-params keyint=120:scenecut=0 -preset veryfast -acodec libmp3lame -ar 44100 -threads 6 -f flv \"{FFmpegOut}\"";

			Task.Run(LaunchFFmpeg);

			if (IsWindows)
			{
				VTarget = new PipedOutTarget_Win(VPipeName);
				ATarget = new PipedOutTarget_Win(APipeName);
			}
			else
			{
				VTarget = new PipedOutTarget_Linux(VPipeName);
				ATarget = new PipedOutTarget_Linux(APipeName);
			}

			return (VTarget, ATarget);
		}

		static async Task LaunchFFmpeg() 
		{
			//Console.WriteLine($"{FFmpegPath} {FFmpegArgs}");
			await Task.Delay(5000);
			System.Diagnostics.Process.Start(FFmpegPath, FFmpegArgs);
		}
	}
}
