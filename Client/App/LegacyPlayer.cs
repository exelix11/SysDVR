using SDL2;
using SysDVR.Client.GUI;
using SysDVR.Client.GUI.Components;
using SysDVR.Client.Targets.Player;
using System;

namespace SysDVR.Client.App
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
            Console.WriteLine(Strings.Starting);
        }

        PlayerManager? ConnectToConsole()
        {
            var conn = GetConnector(CommandLine);

            if (conn is not null)
            {
                conn.OnMessage += Console.WriteLine;
                var device = conn.ConnectForPlayer().GetAwaiter().GetResult();

                device.OnErrorMessage += Console.WriteLine;
                device.OnFatalError += Console.WriteLine;
                return device;
            }

            return null;
        }

        public override void Initialize()
        {
            
        }

        override public void Entrypoint()
        {
            var conn = ConnectToConsole();

            if (conn is null)
                return;

            var hasVideo = conn.HasVideo;

            if (hasVideo)
            {
                sdlCtx.CreateWindow(CommandLine.WindowTitle);

                if (CommandLine.LaunchFullscreen)
                {
                    sdlCtx.SetFullScreen(true);
                    sdlCtx.ShowCursor(false);
                }
            }

            player = new PlayerCore(conn);

            if (conn.HasVideo && player.GetChosenDecoder() is var decoder)
                Console.WriteLine(decoder);

            player.ResolutionChanged();
            player.Start();

            if (hasVideo)
            {
                Console.WriteLine(Strings.Started);

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
                Console.WriteLine(Strings.AudioOnly);
                Console.ReadLine();
            }

        break_main_loop:
            player.Destroy();
            sdlCtx.DestroyWindow();
        }
    }
}
