using SysDVR.Client.Core;
using SysDVR.Client.Sources;
using System;
using System.Linq;
using System.Threading;

namespace SysDVR.Client
{
	internal abstract class BaseStandaloneClient
	{
		internal readonly StringTable.LegacyPlayerTable Strings = Program.Strings.LegacyPlayer;
		protected DvrUsbContext? usb;

		protected DeviceInfo? FindUsbDevice(string? wantedSerial, int attempts)
		{
			usb ??= new DvrUsbContext(Program.Options.UsbLogging);

			for (int i = 0; i < attempts; i++)
			{
				using (var dev = usb.FindSysdvrDevices())
				{
					if (dev.Count > 0)
					{
						if (wantedSerial is not null)
						{
							var target = dev.FirstOrDefault(x => x.Info.Serial.EndsWith(wantedSerial, StringComparison.OrdinalIgnoreCase));
							if (target is not null)
							{
								dev.ExcludeFromDispose(target);
								return target.Info;
							}
						}
						else
						{
							if (dev.Count > 1)
							{
								Console.WriteLine(Strings.MultipleDevicesLine1);
								dev.Select(x => x.Info.Serial).ToList().ForEach(Console.WriteLine);
								Console.WriteLine(Strings.MultipleDevicesLine2);
							}

							var res = dev.First();
							dev.ExcludeFromDispose(res);
							return res.Info;
						}
					}
				}

				Console.WriteLine(Strings.DeviceNotFoundRetry);
				Thread.Sleep(5000);
			}

			Console.WriteLine(Strings.UsbDeviceNotFound);
			return null;
		}

		protected DeviceConnector? GetConnector(CommandLineOptions commandLine)
		{
			DeviceInfo? target = null;

			if (commandLine.StreamingMode == CommandLineOptions.StreamMode.None)
			{
				Console.WriteLine(Strings.NoStreamingError);
				return null;
			}
			else if (commandLine.StreamingMode == CommandLineOptions.StreamMode.Network && string.IsNullOrWhiteSpace(commandLine.NetStreamHostname))
			{
				Console.WriteLine(Strings.TcpWithoutIPError);
				return null;
			}
			else if (commandLine.StreamingMode == CommandLineOptions.StreamMode.Network)
				target = DeviceInfo.ForIp(commandLine.NetStreamHostname);
			else if (commandLine.StreamingMode == CommandLineOptions.StreamMode.Usb)
				target = FindUsbDevice(commandLine.ConsoleSerial, 3);
			else if (commandLine.StreamingMode == CommandLineOptions.StreamMode.Stub)
				target = DeviceInfo.Stub();

			if (target is not null)
			{
				if (!target.IsProtocolSupported)
				{
					Console.WriteLine(Program.Strings.Connection.DeviceNotCompatible);
					target.Dispose();
					return null;
				}

				return new DeviceConnector(target, new(), Program.Options.Streaming);
			}

			Console.WriteLine(Strings.CommandLineError);

			return null;
		}
	}
}
