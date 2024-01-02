using SDL2;
using SysDVR.Client.Core;
using SysDVR.Client.GUI;
using SysDVR.Client.GUI.Components;
using SysDVR.Client.Sources;
using SysDVR.Client.Targets.Player;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SysDVR.Client
{
    // the legacy player is a standalone player that uses its own SDL context without imgui
    // it is used for playback without any additional GUI features
    // Additionally this may be used for adding back command-line only feature that were cut with V 6.0
    public class LegacyPlayer
    {
        readonly CommandLineOptions CommandLine;
        SDLContext sdlCtx => Program.SdlCtx;

        PlayerCore? player;
		DvrUsbContext? usb;

		public LegacyPlayer(CommandLineOptions args)
        {
            CommandLine = args;
            Console.WriteLine("Starting in legacy mode...");
        }

        DeviceInfo? FindUsbDevice(string? wantedSerial, int attempts)
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
                                return target.Info;
                        }
                        else
                        {
                            if (dev.Count > 1)
                            {
                                Console.WriteLine("Multiple devices found:");
                                dev.Select(x => x.Info.Serial).ToList().ForEach(Console.WriteLine);
                                Console.WriteLine("THe first one will be used, you can select a specific one by using the --serial option in the command line");
                            }

                            return dev.First().Info;
                        }
                    }
                }

				Console.WriteLine("Device not found, trying again in 5 seconds...");
				Thread.Sleep(5000);
			}

            Console.WriteLine("No usb device found");
            return null;
		}

		PlayerManager? ConnectToConsole()
        {
            DeviceInfo? target = null;

            if (CommandLine.StreamingMode == CommandLineOptions.StreamMode.None)
            {
                Console.WriteLine("No streaming has been requested. For help, use --help");
                return null;
            }
            else if (CommandLine.StreamingMode == CommandLineOptions.StreamMode.Network && string.IsNullOrWhiteSpace(CommandLine.NetStreamHostname))
            {
                Console.WriteLine("TCP mode without a target IP is not supported in legacy mode, use --help for help");
                return null;
            }
            else if (CommandLine.StreamingMode == CommandLineOptions.StreamMode.Network)
                target = DeviceInfo.ForIp(CommandLine.NetStreamHostname);
            else if (CommandLine.StreamingMode == CommandLineOptions.StreamMode.Usb)
                target = FindUsbDevice(CommandLine.ConsoleSerial, 3);
            else if (CommandLine.StreamingMode == CommandLineOptions.StreamMode.Stub)
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

                var conn = new DeviceConnector(target, new(), Program.Options.Streaming);

				conn.OnMessage += Conn_OnMessage;
                
                return conn.ConnectForPlayer().GetAwaiter().GetResult();
            }

			Console.WriteLine("Invalid command line. The legacy player only supports a subset of options, use --help for help");
			return null;
		}

		public void EntryPoint()
        {
            var conn = ConnectToConsole();

            if (conn is null)
                return;

            var hasVideo = conn.HasVideo;

            if (hasVideo)
            { 
				sdlCtx.CreateWindow(CommandLine.WindowTitle);
				if (CommandLine.LaunchFullscreen)
					sdlCtx.SetFullScreen(true);
			}

			conn.OnErrorMessage += Conn_OnErrorMessage;
			conn.OnFatalError += Conn_OnFatalError;
            player = new PlayerCore(conn);

            if (conn.HasVideo && player.GetChosenDecoder() is var decoder)
                Console.WriteLine(decoder);

            player.Start();

            if (hasVideo)
            {
                Console.WriteLine("Starting to stream, press F11 for full screen.");
                Console.WriteLine("Press return to print stats.");

                while (true)
                {
                    GuiMessage msg = GuiMessage.None;
                    while ((msg = sdlCtx.PumpEvents(out var evt)) != GuiMessage.None)
                    {
                        if (msg is GuiMessage.BackButton or GuiMessage.Quit)
                            goto break_main_loop;
                        else if (msg is GuiMessage.Resize)
                        {
                            player.ResolutionChanged();
                            sdlCtx.ClearScreen();
                        }
                        else if (msg is GuiMessage.KeyUp)
                        {
                            if (evt.key.keysym.scancode == SDL.SDL_Scancode.SDL_SCANCODE_RETURN)
                            {
                                Console.WriteLine(player.GetDebugString());
                            }
                        }
                    }

                    player.DrawLocked();
                    sdlCtx.Render();
                }
            }
            else 
            {
                Console.WriteLine("No video output needed, press return to quit");
                Console.ReadLine();
            }

        break_main_loop:
            player.Destroy();
            sdlCtx.DestroyWindow();
		}

        private void Conn_OnMessage(string obj)
		{
			Console.WriteLine(obj);
		}

		private void Conn_OnFatalError(Exception obj)
		{
			Console.WriteLine(obj);
		}

		private void Conn_OnErrorMessage(string obj)
		{
			Console.WriteLine(obj);
		}
	}
}
