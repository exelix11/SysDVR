using libusbK;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;

namespace NetStream
{
	class Program
	{
		static readonly byte[] SPS = { 0x00, 0x00, 0x00, 0x01, 0x67, 0x64, 0x0C, 0x20, 0xAC, 0x2B, 0x40, 0x28, 0x02, 0xDD, 0x35, 0x01, 0x0D, 0x01, 0xE0, 0x80 };
		static readonly byte[] PPS = { 0x00, 0x00, 0x00, 0x01, 0x68, 0xEE, 0x3C, 0xB0 };

		static readonly byte[] REQMagic = BitConverter.GetBytes(0xDEADCAFE);
		static readonly byte[] REQMagic_AUDIO = BitConverter.GetBytes(0xBA5EBA11);
		const int VbufSz = 0x32000;

		interface IOutTarget : IDisposable
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

		static UsbDevStream GetDevice() 
		{
			var pat = new KLST_PATTERN_MATCH { DeviceID = @"USB\VID_057E&PID_3000" };
			var lst = new LstK(0, ref pat);
			lst.MoveNext(out var dinfo);
			return new UsbDevStream(dinfo);
		}

		static bool StreamLoop(IOutTarget VTarget, UsbDevStream stream, IOutTarget ATarget = null)
		{
			bool FirstPacket = true;
			byte[] SizeBuf = new byte[4];
			ArrayPool<byte> sh = ArrayPool<byte>.Create();
			byte[] data = null;

			uint ReadToSharedArray() 
			{
				stream.Read(SizeBuf);
				var size = BitConverter.ToUInt32(SizeBuf);
				if (size > VbufSz || size == 0) return size;

				data = sh.Rent((int)size);
				stream.Read(data, 0, (int)size);

				return size;
			}

			void FreeBuffer() 
			{
				if (data != null) sh.Return(data);
				data = null;
			}

			try
			{
				while (true)
				{
					if (Console.KeyAvailable) return true;
					
					stream.Write(ATarget == null ? REQMagic : REQMagic_AUDIO);

					var size = ReadToSharedArray();
					if (size > VbufSz || size == 0)
					{
						Console.WriteLine($"Discarding vid packet of size {size}");
					}
					else
					{
						if (FirstPacket)
						{
							if (!data.Matches(0, SPS, 0, 5))
							{
								Console.WriteLine("Warning: Couldn't find SPS NAL, the built-in one will be used. Image artifacts may occour");
								VTarget.SendData(SPS);
								VTarget.SendData(PPS);
							}
							FirstPacket = false;
						}

						VTarget.SendData(data, 0, (int)size);
						FreeBuffer();
					}

					if (ATarget == null) continue;
					
					size = ReadToSharedArray();
					if (size > VbufSz || size == 0)
					{
						Console.WriteLine($"Discarding aud packet of size {size}");
					}
					else
					{
						ATarget.SendData(data, 0, (int)size);
						FreeBuffer();
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("There was an exception: " + ex.ToString());
				FreeBuffer();
			}
			return false;
		}

		static void Main(string[] args)
		{
			if (args.Length < 3)
			{
				Console.WriteLine("Usage: \r\n" +
				"NetStream <video method> <video file/port> <audio method> <audio file/port>\r\n" +
				"Method is either file, tcp or null (only for audio)\r\n" +
				"Note that tcp initialization will block till a program connects\r\n\r\n" +
				"Useful commands: \r\n" +
				"mpv tcp://localhost:1337 --no-correct-pts --fps=30 --cache=no --cache-secs=0\r\n" +
				"mpv tcp://localhost:1338 --no-video --demuxer=rawaudio");
				return;
			}

			while (true)
			{
				//IVideoTarget Target = new OutFileTarget("F:/vid.264");
				IOutTarget VTarget = null;
				IOutTarget ATarget = null;

				if (args[0] == "file")
					VTarget = new OutFileTarget(args[1]);
				else if (args[0] == "tcp")
					VTarget = new TCPTarget(System.Net.IPAddress.Any, int.Parse(args[1]));
				else throw new Exception("Unknown video method");

				if (args[2] == "file")
					ATarget = new OutFileTarget(args[3]);
				else if (args[2] == "tcp")
					ATarget = new TCPTarget(System.Net.IPAddress.Any, int.Parse(args[3]));
				else if (args[2] == "null")
					ATarget = null;
				else throw new Exception("Unknown audio method");

				var stream = GetDevice();
				var res = StreamLoop(VTarget, stream, ATarget);

				VTarget?.Dispose();
				ATarget?.Dispose();

				if (res) break;
			}
		}
	}

	static class Exten 
	{
		public static bool Matches(this byte[] arr, byte[] Magic) =>
			arr.Matches(0, Magic, 0, Magic.Length);

		public static bool Matches(this byte[] arr, int arrOfset, byte[] Magic, int MagicOffset, int length)
		{
			for (int i = 0; i < length; i++)
				if (arr[arrOfset + i] != Magic[MagicOffset + i])
					return false;
			return true;
		}
	}

}
