using ImGuiNET;
using SysDVR.Client.Core;
using SysDVR.Client.GUI.Components;
using SysDVR.Client.Platform;
using SysDVR.Client.Targets;
using SysDVR.Client.Targets.FileOutput;
using SysDVR.Client.Targets.Player;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using static SDL2.SDL;

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

    internal class PlayerCore
    {
        internal readonly AudioPlayer? Audio;
        internal readonly VideoPlayer? Video;
        internal readonly PlayerManager Manager;

        readonly FramerateCounter fps = new();

        SDL_Rect DisplayRect = new SDL_Rect();

        public PlayerCore(PlayerManager manager)
        {
            Manager = manager;

            // SyncHelper is disabled if there is only a single stream
            // Note that it can also be disabled via a --debug flag and this is handled by the constructor
            var sync = new StreamSynchronizationHelper(manager.HasAudio && manager.HasVideo);

            if (manager.HasVideo)
            {
                Video = new(Program.Options.DecoderName, Program.Options.HardwareAccel);
                Video.Decoder.SyncHelper = sync;
                manager.VideoTarget.UseContext(Video.Decoder);

                InitializeLoadingTexture();

                fps.Start();
            }

            if (manager.HasAudio)
            {
                Audio = new(manager.AudioTarget);
                manager.AudioTarget.Volume = Program.Options.DefaultVolume / 100f;
            }

            manager.UseSyncManager(sync);
        }

        public void Start()
        {
            Manager.Begin();
            Audio?.Resume();
        }

        public void Destroy()
        {
            Manager.Stop().GetAwaiter().GetResult();

            // Dispose of unmanaged resources
            Audio?.Dispose();
            Video?.Dispose();

            Manager.Dispose();
        }

        public void ResolutionChanged()
        {
            const double Ratio = (double)StreamInfo.VideoWidth / StreamInfo.VideoHeight;

            var w = (int)Program.SdlCtx.WindowSize.X;
            var h = (int)Program.SdlCtx.WindowSize.Y;

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

        int debugFps = 0;
        public string GetDebugString()
        {
            var sb = new StringBuilder();

            if (fps.GetFps(out var f))
                debugFps = f;

            sb.AppendLine($"Video fps: {debugFps} DispRect {DisplayRect.x} {DisplayRect.y} {DisplayRect.w} {DisplayRect.h}");
            sb.AppendLine($"Video pending packets: {Manager.VideoTarget?.Pending}");
            sb.AppendLine($"IsCompatibleAudioStream: {Manager.IsCompatibleAudioStream}");
            return sb.ToString();
        }

        public string? GetChosenDecoder()
        {
            if (Program.Options.DecoderName is not null)
            {
                if (Video.DecoderName != Program.Options.DecoderName)
                {
                    return string.Format(Program.Strings.Player.CustomDecoderError, Program.Options.DecoderName, Video.DecoderName);
                }
                else
                {
                    return string.Format(Program.Strings.Player.CustomDecoderEnabled, Program.Options.DecoderName);
                }
            }

            if (Video.AcceleratedDecotr)
            {
                return string.Format(Program.Strings.Player.CustomDecoderEnabled, Video.DecoderName);
            }

            return null;
        }

        // For imgui usage, this function draws the current frame
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool DrawAsync()
        {
            if (Video is null)
                return false;

            if (Video.DecodeFrame())
            {
                fps.OnFrame();
            }

            // Bypass imgui for this
            SDL_RenderCopy(Program.SdlCtx.RendererHandle, Video.TargetTexture, ref Video.TargetTextureSize, ref DisplayRect);

            // Signal we're presenting something to SDL to kick the decoding thread
            // We don't care if we didn't actually decode anything we just do it here
            // to do this on every vsync to avoid arbitrary sleeps on the other side
            Video.Decoder.OnFrameEvent.Set();

            return true;
        }

        // For legacy player usage, this locks the thread until the next frame is ready
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool DrawLocked()
        {
            if (Video is null)
                return false;

            if (!Video.DecodeFrame())
                return false;

            SDL_RenderCopy(Program.SdlCtx.RendererHandle, Video.TargetTexture, ref Video.TargetTextureSize, ref DisplayRect);
            Video.Decoder.OnFrameEvent.Set();

            return true;
        }

        private unsafe void InitializeLoadingTexture()
        {
            Program.SdlCtx.BugCheckThreadId();

            // This hardcodes YUV dats
            if (Video.TargetTextureFormat != SDL_PIXELFORMAT_IYUV)
                return;

            byte[] data = null;

            try
            {
                data = Resources.ReadResource(Resources.LoadingImage);
            }
            catch
            {
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
                    (nint)ptr, 1280, (nint)(ptr + 0xE1000), 640, (nint)(ptr + 0x119400), 640);
        }
    }

    internal class PlayerView : View
    {
        public readonly StringTable.PlayerTable Strings = Program.Strings.Player;

        readonly bool HasAudio;
        readonly bool HasVideo;

        readonly PlayerCore player;

        bool OverlayAlwaysShowing = false;

        List<PendingUiNotif> notifications = new();

        Gui.CenterGroup uiOptCenter;
        Gui.CenterGroup quitOptCenter;
        Gui.Popup quitConfirm = new(Program.Strings.Player.ConfirmQuitPopupTitle);
        Gui.Popup fatalError = new(Program.Strings.General.PopupErrorTitle);

        bool drawUi;
        string fatalMessage;

        public bool IsRecording => videoRecorder is not null;
        string recordingButtonText = Program.Strings.Player.StartRecording;
        Mp4Output? videoRecorder;

        readonly string volumePercentFormat;

        void MessageUi(string message)
        {
            Console.WriteLine(message);
            notifications.Add(new PendingUiNotif(message));
        }

        public override void Created()
        {
            player.Start();
            base.Created();
        }

        public override void DrawDebug()
        {
            ImGui.Text(player.GetDebugString());
        }

        void ShowPlayerOptionMessage()
        {
            var dec = player.GetChosenDecoder();
            if (dec is not null)
                MessageUi(dec);
        }

        public PlayerView(PlayerManager manager)
        {
            // Adaptive rendering causes a lot of stuttering, for now avoid it in the video player
            RenderMode =
                Program.Options.UncapStreaming ? FramerateCapOptions.Uncapped() :
                FramerateCapOptions.Target(36);

            Popups.Add(quitConfirm);
            Popups.Add(fatalError);

            HasVideo = manager.HasVideo;
            HasAudio = manager.HasAudio;

            if (!HasAudio && !HasVideo)
                throw new Exception("Can't start a player with no streams");

            manager.OnFatalError += Manager_OnFatalError;
            manager.OnErrorMessage += Manager_OnErrorMessage;

            player = new PlayerCore(manager);

            if (HasVideo)
                ShowPlayerOptionMessage();

            if (!HasVideo)
                OverlayAlwaysShowing = true;

            drawUi = OverlayAlwaysShowing;

            if (Program.Options.PlayerHotkeys && !Program.IsAndroid) // Android is less likely to have a keyboard so don't show the hint. The hotkeys still work.
                MessageUi(Strings.Shortcuts);

            // Convert C# format to ImGui format
            volumePercentFormat = Strings.VolumePercent
                .Replace("%", "%%") // escape the % sign
                .Replace("{0}", "%d"); // replace the format specifier
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
            // Cursor is hidden only during full screen and when there are no other popups
            bool shouldHideCursor = Program.SdlCtx.IsFullscreen;

            if (!Gui.BeginWindow("Player", ImGuiWindowFlags.NoBackground))
            {
                // Nothing to do here
                Program.SdlCtx.ShowCursor(true);
                return;
            }

            if (!HasVideo)
            {
                // When there is no video, show the cursor.
                shouldHideCursor = false;

                Gui.H2();
                Gui.CenterText(Strings.AudioOnlyMode);
                Gui.PopFont();
            }

            for (int i = 0; i < notifications.Count; i++)
            {
                var notif = notifications[i];
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1, 0, 0, 1));
                ImGui.TextWrapped(notif.Text);
                ImGui.PopStyleColor();
                if (notif.ShouldRemove)
                {
                    notifications.RemoveAt(i);
                    notif.Dispose();
                }
            }

            if (drawUi)
            {
                shouldHideCursor = false;
                DrawOverlayMenu();
            }

            if (!OverlayAlwaysShowing)
                DrawOverlayToggleArea();

            if (quitConfirm.Begin())
            {
                shouldHideCursor = false;
                DrawQuitModal();
            }

            if (fatalError.Begin(ImGui.GetIO().DisplaySize * 0.95f))
            {
                shouldHideCursor = false;

                ImGui.TextWrapped(fatalMessage);
                if (Gui.CenterButton(GeneralStrings.PopupCloseButton))
                    Program.Instance.PopView();

                Gui.MakeWindowScrollable();
                ImGui.EndPopup();
            }

            ImGui.End();

            Program.SdlCtx.ShowCursor(!shouldHideCursor);
        }

        public override void OnKeyPressed(SDL_Keysym key)
        {
            if (!Program.Options.PlayerHotkeys)
                return;

            // Handle hotkeys
            if (key.sym == SDL_Keycode.SDLK_s)
                ButtonScreenshot();
            if (key.sym == SDL_Keycode.SDLK_r)
                ButtonToggleRecording();
            if (key.sym == SDL_Keycode.SDLK_f)
                Program.SdlCtx.SetFullScreen(!Program.SdlCtx.IsFullscreen);
            if (key.sym == SDL_Keycode.SDLK_UP && player.Manager.AudioTarget is not null)
                player.Manager.AudioTarget.Volume += 0.1f;
            if (key.sym == SDL_Keycode.SDLK_DOWN && player.Manager.AudioTarget is not null)
                player.Manager.AudioTarget.Volume -= 0.1f;
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
            var rect = new ImRect()
            {
                Min = ImGui.GetWindowPos(),
                Max = ImGui.GetWindowPos() + ImGui.GetWindowSize(),
            };

            var id = ImGui.GetID("##TepToReveal");
            ImGui.KeepAliveID(id);

            if (ImGui.ButtonBehavior(rect, id, out _, out _, ImGuiButtonFlags.MouseButtonLeft) || ImGui.IsKeyPressed(ImGuiKey.Space))
                drawUi = !drawUi;
        }

        void DrawVolumeSlider(float x, float width)
        {
            if (player.Manager.AudioTarget is not null)
            {
                var vol = (int)(player.Manager.AudioTarget.Volume * 100);
                var volnew = vol;
                ImGui.SetCursorPosX(x);
                ImGui.PushItemWidth(width);
                ImGui.SliderInt("##VolumeSlider", ref volnew, 0, 100, volumePercentFormat);
                if (vol != volnew)
                    player.Manager.AudioTarget.Volume = volnew / 100f;
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
                    if (ImGui.Button(Strings.TakeScreenshot, btnsize)) ButtonScreenshot();
                }

                ImGui.SetCursorPosX(center);
                if (ImGui.Button(recordingButtonText, btnsize)) ButtonToggleRecording();

                ImGui.SetCursorPosX(center);
                if (ImGui.Button(Strings.StopStreaming, btnsize)) ButtonQuit();

                if (Program.Options.Debug.Log)
                {
                    ImGui.SetCursorPosX(center);
                    if (ImGui.Button(Strings.DebugInfo, btnsize)) ButtonStats();
                }

                ImGui.SetCursorPosX(center);
                if (ImGui.Button(Strings.EnterFullScreen, btnsize)) ButtonFullscreen();

                ImGui.NewLine();
                DrawVolumeSlider(center, btnwidth);
            }
            else
            {
                OverlayY = OverlayY * 4 / 6;
                var spacing = ImGui.GetStyle().ItemSpacing.X * 3;

                ImGui.SetCursorPosY(OverlayY + ImGui.GetStyle().WindowPadding.Y);

                uiOptCenter.StartHere();
                if (HasVideo)
                {
                    if (ImGui.Button(Strings.TakeScreenshot)) ButtonScreenshot();
                    ImGui.SameLine();
                }
                if (ImGui.Button(recordingButtonText)) ButtonToggleRecording();
                ImGui.SameLine(0, spacing);
                if (ImGui.Button(Strings.StopStreaming)) ButtonQuit();
                ImGui.SameLine(0, spacing);

                if (Program.Options.Debug.Log)
                {
                    if (ImGui.Button(Strings.DebugInfo)) ButtonStats();
                    ImGui.SameLine();
                }

                if (ImGui.Button(Strings.EnterFullScreen)) ButtonFullscreen();
                uiOptCenter.EndHere();

                ImGui.NewLine();
                var w = ImGui.GetWindowSize().X;
                DrawVolumeSlider(w / 4, w / 2);
            }

            if (!OverlayAlwaysShowing)
            {
                ImGui.SetCursorPosY(ImGui.GetWindowSize().Y - ImGui.CalcTextSize("A").Y - ImGui.GetStyle().WindowPadding.Y);
                Gui.CenterText(Strings.HideOverlayLabel);
            }

            ImGui.GetBackgroundDrawList().AddRectFilled(new(0, OverlayY), ImGui.GetWindowSize(), 0xe0000000);
        }

        void DrawQuitModal()
        {
            ImGui.Text(Strings.ConfirmQuitLabel);
            ImGui.Separator();

            var w = ImGui.GetWindowSize().X / 4;
            quitOptCenter.StartHere();

            if (ImGui.Button(GeneralStrings.YesButton, new(w, 0)))
            {
                quitConfirm.RequestClose();
                Program.Instance.PopView();
            }

            ImGui.SameLine();

            if (ImGui.Button(GeneralStrings.NoButton, new(w, 0)))
                quitConfirm.RequestClose();

            quitOptCenter.EndHere();

            ImGui.EndPopup();
        }

        void ScreenshotToClipboard()
        {
            if (!Program.IsWindows)
                throw new Exception("Screenshots to clipboard are only supported on windows");

            using (var cap = SDLCapture.CaptureTexture(player.Video.TargetTexture))
                Platform.Specific.Win.WinClipboard.CopyCapture(cap);

            MessageUi(Strings.ScreenshotSavedToClip);
        }

        void ScreenshotToFile()
        {
            var path = Program.Options.GetFilePathForScreenshot();
            SDLCapture.ExportTexture(player.Video.TargetTexture, path);
            MessageUi(string.Format(Strings.ScreenshotSaved, path));
        }

        void ButtonScreenshot()
        {
            try
            {
                if (Program.IsWindows)
                {
                    var clip = Program.Options.Windows_ScreenToClip;
                    // shift inverts the clipboard flag
                    if (Program.Instance.ShiftDown)
                        clip = !clip;

                    if (clip)
                    {
                        ScreenshotToClipboard();
                        return;
                    }
                }

                ScreenshotToFile();
            }
            catch (Exception ex)
            {
                MessageUi($"{GeneralStrings.ErrorMessage} {ex}");
                Console.WriteLine(ex);
#if ANDROID_LIB
                MessageUi(Strings.AndroidPermissionError);
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

                    player.Manager.ChainTargets(v, a);

                    recordingButtonText = Strings.StopRecording;
                    MessageUi(string.Format(Strings.RecordingStartedMessage, videoFile));
                }
                catch (Exception ex)
                {
                    MessageUi($"{GeneralStrings.ErrorMessage} {ex}");
                    videoRecorder?.Dispose();
                    videoRecorder = null;
                }
            }
            else
            {
                player.Manager.UnchainTargets(videoRecorder.VideoTarget, videoRecorder.AudioTarget);
                videoRecorder.Stop();
                videoRecorder.Dispose();
                videoRecorder = null;
                recordingButtonText = Strings.StartRecording;
                MessageUi(Strings.RecordingSuccessMessage);
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
            Program.SdlCtx.SetFullScreen(!Program.SdlCtx.IsFullscreen);
        }

        unsafe public override void RawDraw()
        {
            base.RawDraw();
            player.DrawAsync();
        }

        public override void ResolutionChanged()
        {
            player.ResolutionChanged();
        }

        public override void Destroy()
        {
            Program.SdlCtx.BugCheckThreadId();

            Program.SdlCtx.ShowCursor(true);

            if (IsRecording)
                ButtonToggleRecording();

            player.Destroy();
            base.Destroy();
        }
    }
}
