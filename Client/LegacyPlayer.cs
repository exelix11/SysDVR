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
    internal class LegacyPlayer : BaseStandaloneClient
	{
        readonly CommandLineOptions CommandLine;
        SDLContext sdlCtx => Program.SdlCtx;

        PlayerCore? player;

		public LegacyPlayer(CommandLineOptions args)
        {
            CommandLine = args;
            Console.WriteLine("Starting in legacy mode...");
        }

		PlayerManager? ConnectToConsole()
        {
            var conn = GetConnector(CommandLine);
            
            if (conn is not null) { 
				conn.OnMessage += Conn_OnMessage;
                return conn.ConnectForPlayer().GetAwaiter().GetResult();
            }

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

			player.ResolutionChanged();
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
