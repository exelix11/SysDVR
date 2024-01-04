using SysDVR.Client.Core;
using SysDVR.Client.Sources;
using SysDVR.Client.Targets.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SysDVR.Client
{
	internal abstract class BaseStandaloneClient
	{
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
								Console.WriteLine("Multiple devices found:");
								dev.Select(x => x.Info.Serial).ToList().ForEach(Console.WriteLine);
								Console.WriteLine("THe first one will be used, you can select a specific one by using the --serial option in the command line");
							}

							var res = dev.First();
							dev.ExcludeFromDispose(res);
							return res.Info;
						}
					}
				}

				Console.WriteLine("Device not found, trying again in 5 seconds...");
				Thread.Sleep(5000);
			}

			Console.WriteLine("No usb device found");
			return null;
		}

		protected DeviceConnector? GetConnector(CommandLineOptions commandLine)
		{
			DeviceInfo? target = null;

			if (commandLine.StreamingMode == CommandLineOptions.StreamMode.None)
			{
				Console.WriteLine("No streaming has been requested. For help, use --help");
				return null;
			}
			else if (commandLine.StreamingMode == CommandLineOptions.StreamMode.Network && string.IsNullOrWhiteSpace(commandLine.NetStreamHostname))
			{
				Console.WriteLine("TCP mode without a target IP is not supported in legacy mode, use --help for help");
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
					Console.WriteLine("The console does not support the streaming protocol");
					Console.WriteLine("You are using different versions of SysDVR-Client and SysDVR on console. Make sure to use latest version on both and reboot your console");
					target.Dispose();
					return null;
				}

				return new DeviceConnector(target, new(), Program.Options.Streaming);
			}

			Console.WriteLine("Invalid command line. The legacy player only supports a subset of options, use --help for help");

			return null;
		}
	}
}
