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
		public void SendData(byte[] data) => SendData(data, 0, data.Length);
		void SendData(byte[] data, int offset, int size);
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

		public void SendData(byte[] data, int offset, int size)
		{
			Vfs.Write(data, offset, size);
		}
	}

	class TCPTarget : IOutTarget
	{
		Socket VidSoc;

		public TCPTarget(System.Net.IPAddress addr, int port)
		{
			var v = new TcpListener(addr, port);
			v.Start();
			VidSoc = v.AcceptSocket();
			v.Stop();
		}

		public void Dispose()
		{
			VidSoc.Close();
			VidSoc.Dispose();
		}

		public void SendData(byte[] data, int offset, int size)
		{
			VidSoc.Send(data, offset, size, SocketFlags.None);
		}
	}

	class StdInTarget : IOutTarget
	{
		Process proc;

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
	}
}
