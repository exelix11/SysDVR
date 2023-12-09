namespace SysDVR.Client;

using ImGuiNET;
using SDL2;
using SysDVR.Client.Core;
using SysDVR.Client.GUI;
using SysDVR.Client.GUI.Components;
using SysDVR.Client.Platform;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading;
using static SDL2.SDL;

public class ClientApp
{
    public static string Version;

    public IntPtr SdlWindow { get; private set; }
    public IntPtr SdlRenderer { get; private set; }

    public bool IsFullscreen { get; private set; } = false;
    public Vector2 WindowSize { get; private set; }
    public float UiScale { get; private set; }

    readonly CommandLineOptions CommandLine;

    public ClientApp(CommandLineOptions args)
    {
        CommandLine = args;

		// Print commandline deprecation warnings
		if (CommandLine.FileDeprecationWarning)
			Console.WriteLine("The --file option has been removed starting from SysDVR 6.0, you can use the gameplay recording feature whitin the new player instead.");
		if (CommandLine.LowLatencyDeprecationWarning)
			Console.WriteLine("The --mpv and --stdout options have been removed starting from SysDVR 6.0, you should use the default player.");
		if (CommandLine.RTSPDeprecationWarning)
			Console.WriteLine("The --rtsp options have been removed starting from SysDVR 6.0, to stram over RTSP use simple network mode from your console.");

		LoadSettings();
        CommandLine.ApplyOptionOverrides();
        ShowDebugInfo = Program.Options.Debug.Log;
	}

#if ANDROID_LIB
    // On android we must manually check for when imgui needs the keyboard and open it
    // TODO: Will this also open the keyboard when there's a physical one connected?
    bool usingTextinput;
#endif

    public ImFontPtr FontH1 { get; private set; }
    public ImFontPtr FontH2 { get; private set; }
    public ImFontPtr FontText { get; private set; }

    public bool IsPortrait { get; private set; }

    public event Action OnExit;

    // View state management
    Stack<View> Views = new();
    View? CurrentView = null;
    ImGuiStyle DefaultStyle;
    bool PendingViewChanges;
    FramerateCap Cap = new();

    // Async actions that must take place on the main thread outside of the rendering loop
    List<Action> PendingActions = new();

    // Detect bugs such as multiple view pushes in a row
    int SDLThreadId;

    // Debug window state
    public bool ShowDebugInfo = false;
    Vector2 PixelSize;
    Vector2 WantedDPIScale;
    bool IsWantedScale;
    bool ShowImguiDemo;

    // SDL functions must only be called from its main thread
    // but it may not always trigger an exception
    // force it to crash so we know there's a bug
    public void BugCheckThreadId() 
    {
        if (SDLThreadId != Thread.CurrentThread.ManagedThreadId)
            throw new InvalidOperationException();
    }

    unsafe void BackupDeafaultStyle()
    {
        DefaultStyle = *ImGui.GetStyle().NativePtr;
    }

    unsafe void RestoreDefaultStyle()
    {
        *ImGui.GetStyle().NativePtr = DefaultStyle;
    }

    // Private view API, has immediate effect
    void HandlePopView() 
    {
        PendingViewChanges = false;

        CurrentView?.LeaveForeground();
        CurrentView?.Destroy();
        CurrentView = null;
        if (Views.Count > 0 ) 
        {
            CurrentView = Views.Pop();
            Cap.SetMode(CurrentView.RenderMode);
            CurrentView?.ResolutionChanged();
            CurrentView?.EnterForeground();
        }
    }

    void HandleReplaceView(View v)
    {
        PendingViewChanges = false;

        CurrentView?.LeaveForeground();
        CurrentView?.Destroy();
        CurrentView = v;
        
        Cap.SetMode(CurrentView.RenderMode);
        CurrentView?.Created();
        CurrentView?.ResolutionChanged();
        CurrentView?.EnterForeground();
    }

    void HandlePushView(View v)
    {
        PendingViewChanges = false;

        if (CurrentView != null)
            Views.Push(CurrentView);

        CurrentView?.LeaveForeground();
        
        CurrentView = v;
        Cap.SetMode(CurrentView.RenderMode);
        CurrentView?.Created();
        CurrentView?.ResolutionChanged();
        CurrentView?.EnterForeground();
    }

    // Post an action to be handled in the main thread
    public void PostAction(Action act) 
    {
        lock (PendingActions)
        {
            PendingActions.Add(act);
        }
    }

    private void ExecutePendingActions() 
    {
        lock (PendingActions)
        {
            for (int i = 0; i < PendingActions.Count; i++)
                PendingActions[i]();
            PendingActions.Clear();
        }
    }

    // Public view management API, these are deferred as they can be called mid-drawing
    public void PushView(View view)
    {
        if (PendingViewChanges)
            throw new Exception("A view action is already scheduled");

        PendingViewChanges = true;
        PostAction(() => HandlePushView(view));
    }

    public void PopView()
    {
        if (PendingViewChanges)
            throw new Exception("A view action is already scheduled");

        PendingViewChanges = true;
        PostAction(HandlePopView);
    }

    public void ReplaceView(View view)
    {
        if (PendingViewChanges)
            throw new Exception("A view action is already scheduled");

        PendingViewChanges = true;
        PostAction(() => HandleReplaceView(view));
    }

    // Called to simulate input when rendering mode is adaptive, we need it so the video decoding thread can force a re-render even when there is no user input active
    public void KickRendering(bool important) 
    {
        Cap.OnEvent(important);
    }

    public void ClearScrren()
    {
        SDL_SetRenderDrawColor(SdlRenderer, 0x0, 0x0, 0x0, 0xFF);
        SDL_RenderClear(SdlRenderer);
    }

    public void SetFullScreen(bool enableFullScreen)
    {
        IsFullscreen = enableFullScreen;
        SDL_SetWindowFullscreen(SdlWindow, enableFullScreen ? (uint)SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP : 0);
        SDL_ShowCursor(enableFullScreen ? SDL_DISABLE : SDL_ENABLE);
    }

    void DebugWindow()
    {
        ImGui.Begin("Info");
        ImGui.Text($"FPS: {ImGui.GetIO().Framerate} Cap {Cap.CapMode}");
        ImGui.Text($"Window Size: {WindowSize} {(IsPortrait ? "Portrait" : "Landscape")}");
        ImGui.Text($"Pixel Size: {PixelSize} DPI Scale: {WantedDPIScale} UiScale: {UiScale}");
        ImGui.Text($"ThreadId: {SDLThreadId} View stack: {Views.Count}");
        ImGui.Checkbox("Show imgui demo", ref ShowImguiDemo);
        CurrentView?.DrawDebug();
        ImGui.End();

        if (ShowImguiDemo)
            ImGui.ShowDemoWindow();
    }

    private void UpdateSize()
    {
        var cur = WindowSize;
        SDL_GetWindowSize(SdlWindow, out int w, out int h);

        if (cur == new Vector2(w, h))
            return;

        WindowSize = new(w, h);
        IsPortrait = w / (float)h < 1.3;

        // Apply scale so the biggest dimension matches 1280
        var oldscale = UiScale;
        var biggest = Math.Max(w, h);
        UiScale = biggest / 1280f;
        if (UiScale != oldscale)
        {
            RestoreDefaultStyle();
            ImGui.GetStyle().ScaleAllSizes(UiScale);
            ImGui.GetIO().FontGlobalScale = UiScale / 2;
        }

        // Scaling workaround for OSX, SDL_WINDOW_ALLOW_HIGHDPI doesn't seem to work
        //if (OperatingSystem.IsMacOS())
        {
            SDL_GetRendererOutputSize(SdlRenderer, out int pixelWidth, out int pixelHeight);
            PixelSize = new(pixelWidth, pixelHeight);

            float dpiScaleX = pixelWidth / (float)w;
            float spiScaleY = pixelHeight / (float)h;
            WantedDPIScale = new(dpiScaleX, spiScaleY);

            IsWantedScale = SDL_RenderSetScale(SdlRenderer, dpiScaleX, spiScaleY) == 0;
        }

        if (cur != WindowSize)
        {
            CurrentView?.ResolutionChanged();
            SDL_RenderClear(SdlRenderer);
        }
    }

    unsafe void UnsafeImguiInitialization()
    {
        ImGui.GetIO().NativePtr->IniFilename = null;
    }

    private void LoadSettings() 
    {
        try
        {
            var set = SystemUtil.LoadSettingsString();
            if (set is null)
                return;

            Program.Options = Options.FromJson(set);
            Console.WriteLine("Settings loaded");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load settings: {ex}");
        }
	}

    internal void Initialize() 
    {
        if (Program.Options.Debug.Log)
		    Console.WriteLine("Initializing app");

		SDL_Init(SDL_INIT_VIDEO | SDL_INIT_AUDIO).AssertZero(SDL_GetError);

        var flags = SDL_image.IMG_InitFlags.IMG_INIT_JPG | SDL_image.IMG_InitFlags.IMG_INIT_PNG;
        SDL_image.IMG_Init(flags).AssertEqual((int)flags, SDL_image.IMG_GetError);

        SDL_SetHint(SDL_HINT_VIDEO_MINIMIZE_ON_FOCUS_LOSS, "0");

        SDL_SetHint(SDL_HINT_RENDER_SCALE_QUALITY, Program.Options.ScaleHintForSDL);
        
        ImGui.CreateContext();

        UnsafeImguiInitialization();
        ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
        ImGui.GetIO().NavVisible = true;

        InitializeFonts();

        BackupDeafaultStyle();
    }

    internal void InitializeFonts() 
    {
        // Fonts are loaded at double the resolution to reduce blurriness when scaling on high DPI
        const int FontMultiplier = 2;
        const int FontTextSize = 30 * FontMultiplier;
        const int FontH1Size = 45 * FontMultiplier;
        const int FontH2Size = 40 * FontMultiplier;

        var fontData = Resources.ReadResouce(Resources.MainFont);
		
        unsafe
		{
			// TODO: Multilanguage support, we need to add unicode ranges of the languages we want to support
			// However this also needs using a font file that includes them, we could try Google Noto but those come as multiple files
			// and we would need to manually sort out which languages we want to support by unicode ranges
			// There is also probably a limit in texture size cause using range 0x0020 to 0xFFFF seems to fail...
            // For now let imgui figure it out on its own
			ushort* fontRange = null; //stackalloc ushort[] { 0x0020, 0x1FFF, 0 };

			fixed (byte* fontPtr = fontData)
            {
                FontText = ImGui.GetIO().Fonts.AddFontFromMemoryTTF(fontPtr, fontData.Length, FontTextSize, null, fontRange);
                FontH1 = ImGui.GetIO().Fonts.AddFontFromMemoryTTF(fontPtr, fontData.Length, FontH1Size, null, fontRange);
                FontH2 = ImGui.GetIO().Fonts.AddFontFromMemoryTTF(fontPtr, fontData.Length, FontH2Size, null, fontRange);

                // fontPtr and FontRange must be kept pinned until the atlases have actually been built or else bad things will happen
                ImGui.GetIO().Fonts.Build();
            }
		}
	}

    internal void PushMainview() 
    {
        // If no streaming has been requested boot into the main menu
        if (CommandLine.StreamingMode == CommandLineOptions.StreamMode.None)
            HandlePushView(new MainView());
        // If mode = network and no IP specified, connect to the first available console
        else if (CommandLine.StreamingMode == CommandLineOptions.StreamMode.Network && string.IsNullOrWhiteSpace(CommandLine.NetStreamHostname))
			HandlePushView(new NetworkScanView(Program.Options.Streaming, CommandLine.ConsoleSerial ?? "")); // ConsoleSerial is null when non specified, make it "" so the network view takes it as a 'cnnect to anything command'
        // If mode = network and IP specified, connect directly to that
		else if (CommandLine.StreamingMode == CommandLineOptions.StreamMode.Network)
			HandlePushView(new ConnectingView(DeviceInfo.ForIp(CommandLine.NetStreamHostname), Program.Options.Streaming));
		else if (CommandLine.StreamingMode == CommandLineOptions.StreamMode.Usb)
            HandlePushView(new UsbDevicesView(Program.Options.Streaming, CommandLine.ConsoleSerial ?? ""));
        else 
        {
            Debugger.Break();
        }
	}

    internal void EntryPoint()
    {
        SDLThreadId = Thread.CurrentThread.ManagedThreadId;

        var title = CommandLine.WindowTitle is null ? 
            "SysDVR-Client" : $"{CommandLine.WindowTitle} - SysDVR-Client";

		SdlWindow = SDL_CreateWindow(title, SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED,
                StreamInfo.VideoWidth, StreamInfo.VideoHeight,
                SDL_WindowFlags.SDL_WINDOW_ALLOW_HIGHDPI | SDL_WindowFlags.SDL_WINDOW_RESIZABLE)
                .AssertNotNull(SDL_GetError);

        var flags = Program.Options.ForceSoftwareRenderer ? SDL_RendererFlags.SDL_RENDERER_SOFTWARE :
            (SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);
        
        SdlRenderer = SDL_CreateRenderer(SdlWindow, -1, flags).AssertNotNull(SDL_GetError);

        SDL_GetRendererInfo(SdlRenderer, out var info);
        Console.WriteLine($"Initialized SDL with {Marshal.PtrToStringAnsi(info.name)} renderer");

        ImGuiSDL2Impl.InitForSDLRenderer(SdlWindow, SdlRenderer);
        ImGuiSDL2Impl.Init(SdlRenderer);

        if (CommandLine.LaunchFullscreen)
			SetFullScreen(true);

        UpdateSize();

        PushMainview();

        while (true)
        {
            ExecutePendingActions();

            if (CurrentView is null)
                break;

            while (SDL_PollEvent(out var evt) != 0)
            {
                Cap.OnEvent(true);

                if (evt.type == SDL_EventType.SDL_QUIT ||
                    (evt.type == SDL_EventType.SDL_WINDOWEVENT && evt.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_CLOSE))
                {
                    goto break_main_loop;
                }
                else if (evt.type == SDL_EventType.SDL_WINDOWEVENT && evt.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED)
                {
                    UpdateSize();
                }
                else if (evt.type == SDL_EventType.SDL_KEYDOWN && evt.key.keysym.sym == SDL_Keycode.SDLK_F11)
                {
                    SetFullScreen(!IsFullscreen);
                }
                else if (evt.type == SDL_EventType.SDL_KEYDOWN && evt.key.keysym.sym is SDL_Keycode.SDLK_ESCAPE or SDL_Keycode.SDLK_AC_BACK)
                {
                    CurrentView.BackPressed();
                }
                else if (evt.type == SDL_EventType.SDL_KEYUP)
                {
                    // At least on Windows keeping a key pressed spams of keydown and textinput events due to the text input behavior
                    // This also affects imgui IO keydown events.
                    // The only event that is guaranteed to fire is the keyup event so we use it to determine when a key has been pressed and then released
                    CurrentView.OnKeyPressed(evt.key.keysym);
                }

                ImGuiSDL2Impl.ProcessEvent(in evt);

#if ANDROID_LIB
                if (ImGui.GetIO().WantTextInput && !usingTextinput) {
                        SDL.SDL_StartTextInput();
                        usingTextinput = true;
                }
                else if (!ImGui.GetIO().WantTextInput && usingTextinput) {
                        SDL.SDL_StopTextInput();
                        usingTextinput = false;
                }
#endif
            }

            if (Cap.Cap())
                continue;

            var view = CurrentView;

            ImGuiSDL2Impl.Renderer_NewFrame();
            ImGuiSDL2Impl.SDL2_NewFrame();
            ImGui.NewFrame();

            CurrentView.Draw();

            if (ShowDebugInfo)
                DebugWindow();

            ImGui.Render();

            CurrentView.RawDraw();

            ImGuiSDL2Impl.RenderDrawData(ImGui.GetDrawData());

            SDL_RenderPresent(SdlRenderer);
        }
    break_main_loop:

        while (CurrentView != null)
        {
            HandlePopView();
        }

        OnExit?.Invoke();

        ImGuiSDL2Impl.Renderer_Shutdown();
        ImGuiSDL2Impl.SDL2_Shutdown();
        // Seems to crash:
        //  ImGui.DestroyContext(ctx);

        SDL_DestroyRenderer(SdlRenderer);
        SDL_DestroyWindow(SdlWindow);
    }
}