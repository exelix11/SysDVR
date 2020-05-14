using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace SysDVRClient
{
	enum StreamKind
	{
		Video,
		Audio
	};

	interface IOutTarget
	{
		void SendData(byte[] data, UInt64 ts) => SendData(data, 0, data.Length, ts);
		void SendData(byte[] data, int offset, int size, UInt64 ts);
	}

	interface IMutliStreamManager 
	{
		IOutTarget Video { get; }
		IOutTarget Audio { get; }

		bool HasAStream => Video != null || Audio != null;

		void Begin();
		void Stop();
	}

	class OutFileTarget : IOutTarget
	{
		FileStream Vfs;

		public OutFileTarget(string fname)
		{
			Vfs = File.Open(fname, FileMode.Create);
		}

		public void Dispose()
		{
			Vfs.Close();
			Vfs.Dispose();
		}

		public void SendData(byte[] data, int offset, int size, UInt64 ts)
		{
			Vfs.Write(data, offset, size);
		}
}

	class TCPTarget : IOutTarget
	{
		Socket Sock;

		System.Net.IPAddress HostAddr;
		int HostPort;

		public TCPTarget(System.Net.IPAddress addr, int port)
		{
			HostAddr = addr;
			HostPort = port;
		}

		private void ReceiveConnection() 
		{
			var v = new TcpListener(HostAddr, HostPort);
			v.Start();
			Console.WriteLine($"Waiting for connection on port {HostPort}...");
			Sock = v.AcceptSocket();
			v.Stop();
		}

		public void SendData(byte[] data, int offset, int size, UInt64 ts)
		{
			try
			{
				Sock.Send(data, offset, size, SocketFlags.None);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"WARNING - Closing socket on port {HostPort} : {ex.Message}");
				Sock.Close();
				ReceiveConnection();
			}
		}
	}

	class StdInTarget : IOutTarget
	{
		Process proc;

		public StdInTarget(string path, string args)
		{
			ProcessStartInfo p = new ProcessStartInfo()
			{
				Arguments = args,
				FileName = path,
				RedirectStandardInput = true,
				RedirectStandardOutput = true
			};
			proc = Process.Start(p);
		}

		private bool FirstTime = true;
		public void SendData(byte[] data, int offset, int size, UInt64 ts)
		{
			if (FirstTime)
			{
				proc.StandardInput.BaseStream.Write(StreamInfo.SPS);
				proc.StandardInput.BaseStream.Write(StreamInfo.PPS);
				FirstTime = false;
			}
				
			proc.StandardInput.BaseStream.Write(data, offset, size);
		}

	}

#if DEBUG
	class LoggingTarget : IOutTarget
	{
		readonly BinaryWriter bin;
		readonly MemoryStream mem = new MemoryStream();
		readonly string filename;

		public LoggingTarget(string filename)
		{
			this.filename = filename;
			bin = new BinaryWriter(mem);
		}

		~LoggingTarget()
		{
			File.WriteAllBytes(filename, mem.ToArray());
		}

		Stopwatch sw = new Stopwatch();

		public void SendData(byte[] data, int offset, int size, UInt64 ts)
		{
			Console.WriteLine($"{filename} - ts: {ts}");
			bin.Write(sw.ElapsedMilliseconds);
			bin.Write(ts);
			bin.Write(data, offset, size);
			sw.Restart();
		}
	}
#endif
}
