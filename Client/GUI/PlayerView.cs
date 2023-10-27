using System;
using System.Collections.Generic;
using System.Threading;
using System.Numerics;
using ImGuiNET;
using SysDVR.Client.Core;
using SysDVR.Client.Targets.Player;
using SysDVR.Client.Platform;
using SysDVR.Client.Targets;

using static SDL2.SDL;
using SysDVR.Client.Targets.FileOutput;

namespace SysDVR.Client.GUI
{
    class PendingUiNotif : IDisposable
    {
        public string Text;
        public bool ShouldRemove;
        
        Timer disposeTimer;

        public PendingUiNotif(string text)
        {
            Text = text;
            ShouldRemove = false;
            disposeTimer = new Timer((_) => ShouldRemove = true, null, 5000, Timeout.Infinite);
        }

        public void Dispose()
        {
            disposeTimer.Dispose();
        }
    }

    internal class PlayerView : View
    {
        readonly bool HasAudio;
        readonly bool HasVideo;

        readonly AudioPlayer? Audio;
        readonly VideoPlayer? Video;

        readonly FramerateCounter fps = new();

        readonly PlayerManager Manager;

        bool OverlayAlwaysShowing = false;

        SDL_Rect DisplayRect = new SDL_Rect();

        List<PendingUiNotif> notifications = new();
        
        Gui.CenterGroup uiOptCenter;
        Gui.Popup quitConfirm = new("Confirm quit");
        Gui.Popup fatalError = new("Fatal error");
        
        bool drawUi;
        string fatalMessage;

        public bool IsRecording => videoRecorder is not null;
        string recordingButtonText = "Start recording";
        Mp4Output? videoRecorder;

        void MessageUi(string message)
        {
            Console.WriteLine(message);
            notifications.Add(new PendingUiNotif(message));
        }

        public override void Created()
        {
            Manager.Begin();
            Audio?.Resume();
            base.Created();
        }

        int fpsval;
        public override void DrawDebug()
        {
            if (fps.GetFps(out var f))
                    fpsval = f;
            
            ImGui.Text($"Video fps: {fpsval} DispRect {DisplayRect.x} {DisplayRect.y} {DisplayRect.w} {DisplayRect.h}");
            ImGui.Text($"Video pending packets: {Manager.VideoTarget?.Pending}");
            ImGui.Text($"IsCompatibleAudioStream: {Manager.IsCompatibleAudioStream}");
        }

        void ShowPlayerOptionMessage() 
        {
            if (Program.Options.DecoderName is not null)
            {
                if (Video.DecoderName != Program.Options.DecoderName)
                {
                    MessageUi($"Decoder {Program.Options.DecoderName} not found, using {Video.DecoderName} instead.");
                }
                else
                {
                    MessageUi($"Using custom decoder {Program.Options.DecoderName}, in case of issues try disabling this option to use the default one.");
                }
            }

            if (Video.AcceleratedDecotr)
            {
                MessageUi($"Using the {Video.DecoderName} hardware accelerated video decoder, in case of issues try to use the default one by disabling this option.");
            }
        }

        public PlayerView(PlayerManager manager)
        {
            Popups.Add(quitConfirm);
            Popups.Add(fatalError);

            Manager = manager;
            HasVideo = manager.HasVideo;
            HasAudio = manager.HasAudio;

            if (!HasAudio && !HasVideo)
                throw new Exception("Can't start a player with no streams");

            manager.OnFatalError += Manager_OnFatalError;
            manager.OnErrorMessage += Manager_OnErrorMessage;

            // SyncHelper is disabled if there is only a single stream
            // Note that it can also be disabled via a --debug flag and this is handled by the constructor
            var sync = new StreamSynchronizationHelper(HasAudio && HasVideo);

            if (HasVideo)
            {
                Video = new(Program.Options.DecoderName, Program.Options.HardwareAccel);
                Video.Decoder.SyncHelper = sync;
                manager.VideoTarget.UseContext(Video.Decoder);

                ShowPlayerOptionMessage();

                InitializeLoadingTexture();

                fps.Start();
            }

            if (HasAudio)
                Audio = new(manager.AudioTarget);

            manager.UseSyncManager(sync);

            if (!HasVideo)
                OverlayAlwaysShowing = true;

            drawUi = OverlayAlwaysShowing;
        }

        private void Manager_OnErrorMessage(string obj) => 
            MessageUi(obj);

        private void Manager_OnFatalError(Exception obj)
        {
            fatalMessage = obj.ToString();
            Popups.Open(fatalError);
        }

        public override void Draw()
        {
            Gui.BeginWindow("Player", ImGuiWindowFlags.NoBackground);

            if (!HasVideo)
            {
                Gui.H2();
                Gui.CenterText("Streaming is set to audio-only mode.");
                ImGui.PopFont();
            }

            for (int i = 0; i < notifications.Count; i++)
            {
                var notif = notifications[i];
                ImGui.PushStyleColor(ImGuiCol.Text, new System.Numerics.Vector4(1, 0, 0, 1));
                ImGui.Text(notif.Text);
                ImGui.PopStyleColor();
                if (notif.ShouldRemove)
                {
                    notifications.RemoveAt(i);
                    notif.Dispose();
                }
            }

            if (drawUi)
                DrawOverlayMenu();

            DrawOverlayToggleArea();

            DrawQuitModal();

            if (fatalError.Begin(ImGui.GetIO().DisplaySize * 0.95f))
            {
                ImGui.TextWrapped(fatalMessage);
                if (Gui.CenterButton("  Ok  "))
                    Program.Instance.PopView();

                Gui.MakeWindowScrollable();
                ImGui.EndPopup();
            }

            ImGui.End();
        }

        public override void OnKeyPressed(SDL_Keysym key)
        {
            // Handle hotkeys
            if (key.sym == SDL_Keycode.SDLK_c)
                ButtonScreenshot();
        }

        public override void BackPressed()
        {
            if (Popups.AnyOpen)
            {
                base.BackPressed();
                return;
            }
            
            Popups.Open(quitConfirm);
        }

        void DrawOverlayToggleArea() 
        {
            if (OverlayAlwaysShowing)
                return;

            var rect = new ImRect()
            {
                Min = ImGui.GetWindowPos(),
                Max = ImGui.GetWindowPos() + ImGui.GetWindowSize(),
            };

            var id = ImGui.GetID("##TepToReveal");
            ImGui.KeepAliveID(id);
            if (ImGui.ButtonBehavior(rect, id, out _, out _, ImGuiButtonFlags.MouseButtonLeft) || ImGui.IsKeyPressed(ImGuiKey.Space))
            {
                drawUi = !drawUi;
            }
        }

        void DrawOverlayMenu() 
        {
            float OverlayY = ImGui.GetWindowSize().Y;

            if (Program.Instance.IsPortrait)
            {
                OverlayY = OverlayY * 6 / 10;
                ImGui.SetCursorPosY(OverlayY + ImGui.GetStyle().WindowPadding.Y);
                
                var width = ImGui.GetWindowSize().X;

                var btnwidth = width * 3 / 6;
                var btnheight = (ImGui.GetWindowSize().Y - ImGui.GetCursorPosY()) / 8;
                var btnsize = new Vector2(btnwidth, btnheight);

                var center = width / 2 - btnwidth / 2;

                if (HasVideo)
                {
                    ImGui.SetCursorPosX(center);
                    if (ImGui.Button("Screenshot", btnsize)) ButtonScreenshot();
                }

                ImGui.SetCursorPosX(center);
                if (ImGui.Button(recordingButtonText, btnsize)) ButtonToggleRecording();

                ImGui.SetCursorPosX(center);
                if (ImGui.Button("Stop streaming", btnsize)) ButtonQuit();

                ImGui.SetCursorPosX(center);
                if (ImGui.Button("Debug info", btnsize)) ButtonStats();

                ImGui.SetCursorPosX(center);
                if (ImGui.Button("Full screen", btnsize)) ButtonFullscreen();
            }
            else
            {
                OverlayY = OverlayY * 5 / 6;
                var spacing = ImGui.GetStyle().ItemSpacing.X * 3;

                ImGui.SetCursorPosY(OverlayY + ImGui.GetStyle().WindowPadding.Y);

                uiOptCenter.StartHere();
                if (HasVideo)
                {
                    if (ImGui.Button("Screenshot")) ButtonScreenshot();
                    ImGui.SameLine();
                }
                if (ImGui.Button(recordingButtonText)) ButtonToggleRecording();
                ImGui.SameLine(0, spacing);
                if (ImGui.Button("Stop streaming")) ButtonQuit();
                ImGui.SameLine(0, spacing);
                if (ImGui.Button("Debug info")) ButtonStats();
                ImGui.SameLine();
                if (ImGui.Button("Full screen")) ButtonFullscreen();
                uiOptCenter.EndHere();
            }

            if (!OverlayAlwaysShowing)
            {
                ImGui.SetCursorPosY(ImGui.GetWindowSize().Y - ImGui.CalcTextSize("A").Y - ImGui.GetStyle().WindowPadding.Y);
                Gui.CenterText("Tap anywhere to hide the overlay");
            }
            
            ImGui.GetBackgroundDrawList().AddRectFilled(new(0, OverlayY), ImGui.GetWindowSize(), 0xe0000000);
        }

        void DrawQuitModal() 
        {
            if (quitConfirm.Begin())
            {
                ImGui.Text("Are you sure you want to quit?");
                ImGui.Separator();

                if (ImGui.Button("Yes"))
                {
                    quitConfirm.RequestClose();
                    Program.Instance.PopView();
                }

                ImGui.SameLine();

                if (ImGui.Button("No"))
                    quitConfirm.RequestClose();

                ImGui.EndPopup();
            }
        }

        void ButtonScreenshot() 
        {
            try 
            {
                var path = Program.Options.GetFilePathForScreenshot();
                SDLCapture.ExportTexture(Video.TargetTexture, path);
                MessageUi("Screenshot saved to " + path);
            }
            catch (Exception ex)
            {
                MessageUi("Error: " + ex.Message);
                Console.WriteLine(ex);
#if ANDROID_LIB
                MessageUi("Make sure you enabled file access permissions to the app !");
#endif
            }
        }

        void ButtonToggleRecording()
        {
            if (videoRecorder is null)
            {
                try 
                {
                    var videoFile = Program.Options.GetFilePathForVideo();

                    Mp4VideoTarget? v = HasVideo ? new() : null;
                    Mp4AudioTarget? a = HasAudio ? new() : null; 

                    videoRecorder = new Mp4Output(videoFile, v, a);
                    videoRecorder.Start();

                    Manager.ChainTargets(v, a);

                    recordingButtonText = "Stop recording";
                    MessageUi("Recording to " + videoFile);
                }
                catch (Exception ex)
                {
                    MessageUi("Failed to start recording: " + ex.ToString());
                }
            }
            else 
            {
                Manager.UnchainTargets(videoRecorder.VideoTarget, videoRecorder.AudioTarget);
                videoRecorder.Stop();
                videoRecorder.Dispose();
                videoRecorder = null;
                recordingButtonText = "Start recording";
                MessageUi("Finished recording");
            }
        }
        
        void ButtonStats()
        {
            Program.Instance.ShowDebugInfo = !Program.Instance.ShowDebugInfo;
        }
        
        void ButtonQuit() 
        {
            BackPressed(); 
        }

        void ButtonFullscreen() 
        {
            Program.Instance.SetFullScreen(!Program.Instance.IsFullscreen);
        }

        unsafe public override void RawDraw()
        {
            base.RawDraw();

            if (!HasVideo)
                return;

            if (Video.DecodeFrame())
            {
                fps.OnFrame();
            }

            // Bypass imgui for this
            SDL_RenderCopy(Program.Instance.SdlRenderer, Video.TargetTexture, ref Video.TargetTextureSize, ref DisplayRect);

            // Signal we're presenting something to SDL to kick the decding thread
            // We don't care if we didn't actually decoded anything we just do it here
            // to do this on every vsync to avoid arbitrary sleeps on the other side
            Video.Decoder.OnFrameEvent.Set();
        }

        public override void ResolutionChanged()
        {
            const double Ratio = (double)StreamInfo.VideoWidth / StreamInfo.VideoHeight;

            var w = (int)Program.Instance.WindowSize.X;
            var h = (int)Program.Instance.WindowSize.Y;

            // TODO: Verify what's the state of this in 6.0
            // Scaling workaround for OSX, SDL_WINDOW_ALLOW_HIGHDPI doesn't seem to work
            //SDL_GetRendererOutputSize(Program.SdlRenderer, out int pixelWidth, out int pixelHeight);
            //float scaleX = pixelWidth / (float)w;
            //float scaleY = pixelHeight / (float)h;
            //SDL_RenderSetScale(SDL.Renderer, scaleX, scaleY);

            if (w >= h * Ratio)
            {
                DisplayRect.w = (int)(h * Ratio);
                DisplayRect.h = h;
            }
            else
            {
                DisplayRect.h = (int)(w / Ratio);
                DisplayRect.w = w;
            }

            DisplayRect.x = w / 2 - DisplayRect.w / 2;
            DisplayRect.y = h / 2 - DisplayRect.h / 2;
        }

        public unsafe override void Destroy()
        {
            Program.Instance.BugCheckThreadId();

            if (IsRecording)
                ButtonToggleRecording();

            Manager.Stop();

            // Dispose of unmanaged resources
            Audio?.Dispose();
            Video?.Dispose();

            Manager.Dispose();
            base.Destroy();
        }      

        private unsafe void InitializeLoadingTexture()
        {
            Program.Instance.BugCheckThreadId();

            // This hardcodes YUV dats
            if (Video.TargetTextureFormat != SDL_PIXELFORMAT_IYUV)
                return;

            byte[] data = null;

            try {
                data = Resources.ReadResouce(Resources.LoadingImage);
            }
            catch {
                // Don't care
            }
            
            if (data == null)
            {
                // Hardcoded buffer size for a 1280x720 YUV texture
                data = new byte[0x1517F0];
                // Fill with YUV white
                data.AsSpan(0, 0xE1000).Fill(0xFF);
                data.AsSpan(0xE1000, 0x119400 - 0xE1000).Fill(0x7F);
                data.AsSpan(0x119400).Fill(0x80);
            }

            fixed (byte* ptr = data)
                SDL_UpdateYUVTexture(Video.TargetTexture, ref Video.TargetTextureSize,
                    (IntPtr)ptr, 1280, (IntPtr)(ptr + 0xE1000), 640, (IntPtr)(ptr + 0x119400), 640);
        }        
    }
}
