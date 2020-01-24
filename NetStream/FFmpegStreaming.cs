using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UsbStream
{
	class FFMpegHost
	{
		protected Process Encoder;
		protected string FFmpegArgs = "";

		protected bool FFmpegRedirOut = false;

		public FFMpegHost(string args) 
		{
			FFmpegArgs = args;
		}

		private void DisposeProc()
		{
			Encoder?.Kill();
			Encoder?.Dispose();
		}

		void prepare() 
		{
			DisposeProc();

			Encoder = new Process();
			Encoder.StartInfo = new ProcessStartInfo()
			{
				FileName = "ffmpeg",
				Arguments = FFmpegArgs,
				RedirectStandardOutput = FFmpegRedirOut,
				CreateNoWindow = FFmpegRedirOut
			};
		}

		public virtual void Kill()
		{
			DisposeProc();
		}

		public virtual void Start()
		{
			prepare();
			Encoder.Start();
		}
	}

	class FFmpegPlayHost : FFMpegHost 
	{
		protected Process Player;
		protected string PlayerName;

		CancellationTokenSource cancellation;
		Thread StdoutForwardThread;

		public FFmpegPlayHost(string playerName, string ffmpegArgs) : base(ffmpegArgs)
		{
			FFmpegRedirOut = true;
			PlayerName = playerName;
		}

		void DisposeProc() 
		{
			Player?.Kill();
			Player?.Dispose();

			cancellation?.Cancel();
			StdoutForwardThread?.Join();
			StdoutForwardThread = null;
		}

		void prepare() 
		{
			DisposeProc();
			Player = new Process();
			Player.StartInfo = new ProcessStartInfo()
			{
				Arguments = "-",
				FileName = PlayerName,
				RedirectStandardInput = true
			};

			cancellation = new CancellationTokenSource();
			StdoutForwardThread = new Thread(() => {

				var token = cancellation.Token;

				var source = Encoder.StandardOutput.BaseStream;
				var target = Player.StandardInput.BaseStream;

				try
				{
					//Equivalent to Stream.CopyTo() but with support for cancellation
					byte[] buffer = new byte[82920];
					int read;
					while (!token.IsCancellationRequested)
					{
						read = source.Read(buffer, 0, 82920);
						target.Write(buffer, 0, read);
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine($"StdoutForwardThread : {ex}");
				}
			});
		}

		public override void Kill()
		{
			base.Kill();
			DisposeProc();
		}

		public override void Start()
		{
			base.Start();
			prepare();
			Player.Start();

			StdoutForwardThread.Start();
		}
	}

	interface IPipe : IDisposable
	{
		Stream GetStream();
		void Reset();
		void WaitForClient();

		string PipeName { get; }
	}

	class WindowsPipe : IPipe
	{
		NamedPipeServerStream pipe;
		public string PipeName { get; private set; }

		public WindowsPipe(string pipeName)
		{
			PipeName = pipeName;
			Reset();
		}

		public void Dispose() => pipe?.Dispose();
		public Stream GetStream() => pipe;

		public void Reset()
		{
			Dispose();
			pipe = new NamedPipeServerStream(PipeName, PipeDirection.Out, 1, PipeTransmissionMode.Byte);
		}

		public void WaitForClient()
		{
			Console.WriteLine($"Waiting for connection on pipe {PipeName}");
			pipe.WaitForConnection();
		}
	}

	//TODO: test on linux
	class LinuxPipe : IPipe
	{
		FileStream pipe;
		public string PipeName { get; private set; }

		public LinuxPipe(string pipeName)
		{
			PipeName = "/tmp/" + pipeName;
			Reset();
		}

		public void Dispose()
		{
			pipe?.Close();
			pipe?.Dispose();
			Mono.Unix.Native.Syscall.unlink(PipeName);
		}

		public Stream GetStream() => pipe;

		public void Reset()
		{
			Dispose();
			Mono.Unix.Native.Syscall.mkfifo(PipeName, Mono.Unix.Native.FilePermissions.ACCESSPERMS);
			pipe = File.Open(PipeName, FileMode.Open, FileAccess.Write);
		}

		public void WaitForClient()
		{
			// Can't wait for connection on linux fifos 
		}
	}

	class PipedOutTarget : IOutTarget
	{
		FFMpegHost Encoder;
		IPipe Pipe;
		Stream PipeStream;

		public event IOutTarget.ClientConnectedDelegate ClientConnected;

		public PipedOutTarget(IPipe pipe, FFMpegHost encoder)
		{
			Pipe = pipe;
			Encoder = encoder;
		}

		public void Dispose()
		{
			Encoder?.Kill();
			Pipe.Dispose();
			PipeStream = null;
		}

		public void SendData(byte[] data, int offset, int size)
		{
			try
			{
				PipeStream.Write(data, offset, size);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"WARNING - pipe {Pipe.PipeName} : {ex.Message}");
				Pipe.Reset();
				InitializeStreaming();
			}
		}

		public void InitializeStreaming()
		{
			Encoder?.Start();
			Pipe.WaitForClient();
			PipeStream = Pipe.GetStream();
			ClientConnected();
		}
	}

	static class FFmpegStreamHelper
	{
		const string VPipeName = "UsbStreamV";
		const string APipeName = "UsbStreamA";

		static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

		static string VPipeFile => IsWindows ? $"//./pipe/{VPipeName}" : $"/tmp/{VPipeName}";
		static string APipeFile => IsWindows ? $"//./pipe/{APipeName}" : $"/tmp/{APipeName}";

		//This will call it from path if availabe
		public static string FFmpegPath = "ffmpeg";

		private static (IOutTarget, IOutTarget) CreateTargets(FFMpegHost host)
		{
			IOutTarget VTarget = null, ATarget = null;
			IPipe VPipe = null, APipe = null;
			
			if (IsWindows)
			{
				VPipe = new WindowsPipe(VPipeName);
				APipe = new WindowsPipe(APipeName);
			}
			else
			{
				VPipe = new LinuxPipe(VPipeName);
				APipe = new LinuxPipe(APipeName);
			}

			//Only one pipe needs to host the ffmpeg process, doesn't really matter which one it is 
			VTarget = new PipedOutTarget(VPipe, host);
			ATarget = new PipedOutTarget(APipe, null);

			return (VTarget, ATarget);
		}

		public static (IOutTarget, IOutTarget) ParseArgsLive(string[] args)
		{
			//For live streaming services we need to have a keyframe at least every two seconds, it's not always the case in grc:d output
			string FFmpegArgs = $"-fflags +genpts -f s16le -ar 48000 -ac 2 -i \"{APipeFile}\" -f h264 -r 30 -framerate 30 -i \"{VPipeFile}\" -vcodec libx264 -pix_fmt yuv420p -b:v 2500k -x264-params keyint=120:scenecut=0 -preset veryfast -acodec libmp3lame -ar 44100 -threads 6 -f flv \"{args[1]}\"";
			if (args.Length > 2) FFmpegPath = args[2];
			FFMpegHost host = new FFMpegHost(FFmpegArgs);
			return CreateTargets(host);
		}

		public static (IOutTarget, IOutTarget) ParseArgsPlay(string[] args)
		{
			//When streaming to a local player we can just passthrough the video data
			string FFmpegArgs = $"-fflags +genpts -f s16le -ar 48000 -ac 2 -i \"{APipeFile}\" -f h264 -r 30 -framerate 30 -i \"{VPipeFile}\" -vcodec copy -acodec libmp3lame -ar 44100 -threads 2 -f flv -";
			if (args.Length > 2) FFmpegPath = args[2];
			FFmpegPlayHost host = new FFmpegPlayHost(args[1], FFmpegArgs);
			return CreateTargets(host);
		}
	}
}
