using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace UsbStream
{
	public interface IOutTarget : IDisposable
	{
		public delegate void ClientConnectedDelegate();
		public event ClientConnectedDelegate ClientConnected;

		public void SendData(byte[] data) => SendData(data, 0, data.Length);
		void SendData(byte[] data, int offset, int size);
		void InitializeStreaming();
	}

	class OutFileTarget : IOutTarget
	{
		FileStream Vfs;

		public event IOutTarget.ClientConnectedDelegate ClientConnected;

		public OutFileTarget(string fname)
		{
			Vfs = File.Open(fname, FileMode.Create);
		}

		public void Dispose()
		{
			Vfs.Close();
			Vfs.Dispose();
		}

		public void SendData(byte[] data, int offset, int size)
		{
			Vfs.Write(data, offset, size);
		}

		public void InitializeStreaming() { ClientConnected(); }
	}

	class TCPTarget : IOutTarget
	{
		Socket Sock;

		System.Net.IPAddress HostAddr;
		int HostPort;

		public event IOutTarget.ClientConnectedDelegate ClientConnected;

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
			ClientConnected();
		}

		public void Dispose()
		{
			Sock.Close();
			Sock.Dispose();
		}

		public void SendData(byte[] data, int offset, int size)
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

		public void InitializeStreaming() => ReceiveConnection();
	}

	class StdInTarget : IOutTarget
	{
		Process proc;

		public event IOutTarget.ClientConnectedDelegate ClientConnected;

		public StdInTarget(string path, string args)
		{
			ProcessStartInfo p = new ProcessStartInfo()
			{
				Arguments = " - " + args,
				FileName = path,
				RedirectStandardInput = true,
				RedirectStandardOutput = true
			};
			proc = Process.Start(p);
		}

		public void Dispose()
		{
			if (!proc.HasExited)
				proc.Kill();
		}

		public void SendData(byte[] data, int offset, int size)
		{
			proc.StandardInput.BaseStream.Write(data, offset, size);
		}

		public void InitializeStreaming() { ClientConnected(); }
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

		public void Dispose()
		{
			File.WriteAllBytes(filename, mem.ToArray());
		}

		Stopwatch sw = new Stopwatch();

		public event IOutTarget.ClientConnectedDelegate ClientConnected;

		public void SendData(byte[] data, int offset, int size)
		{
			throw new NotImplementedException();	
		}

		public void SendData(byte[] data, int offset, int size, UInt64 ts)
		{
			Console.WriteLine($"{filename} - ts: {ts}");
			bin.Write(sw.ElapsedMilliseconds);
			bin.Write(ts);
			bin.Write(data, offset, size);
			sw.Restart();
		}

		public void InitializeStreaming()
		{

		}
	}
#endif
}
