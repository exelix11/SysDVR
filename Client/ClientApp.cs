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

public class ClientApp
{
    public static string Version;

    public float UiScale { get; private set; }

    readonly CommandLineOptions CommandLine;

    readonly SDLContext sdlCtx;

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

        sdlCtx = new();
        Program.SdlCtx = sdlCtx;
	}

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

    // Debug window state
    public bool ShowDebugInfo = false;
    bool ShowImguiDemo;

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

    void DebugWindow()
    {
        ImGui.Begin("Info");
        ImGui.Text($"FPS: {ImGui.GetIO().Framerate} Cap {Cap.CapMode}");
		ImGui.Text(sdlCtx.GetDebugInfo());
		ImGui.Text($"scale: {UiScale} mode: {(IsPortrait ? "Portrait" : "Landscape")} stack: {Views.Count}");
		ImGui.Checkbox("Show imgui demo", ref ShowImguiDemo);
        CurrentView?.DrawDebug();
        ImGui.End();

        if (ShowImguiDemo)
            ImGui.ShowDemoWindow();
    }

    private void UpdateSize()
    {
        var w = sdlCtx.WindowSize.X;
        var h = sdlCtx.WindowSize.Y;

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

        CurrentView?.ResolutionChanged();
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
        var title = CommandLine.WindowTitle is null ? 
            "SysDVR-Client" : $"{CommandLine.WindowTitle} - SysDVR-Client";

        sdlCtx.CreateWindow(title);

        ImGuiSDL2Impl.InitForSDLRenderer(sdlCtx.WindowHandle, sdlCtx.RendererHandle);
        ImGuiSDL2Impl.Init(sdlCtx.RendererHandle);

        if (CommandLine.LaunchFullscreen)
			sdlCtx.SetFullScreen(true);

        UpdateSize();

        PushMainview();

        while (true)
        {
            ExecutePendingActions();

            if (CurrentView is null)
                break;

			GuiMessage msg = GuiMessage.None;
			while ((msg = sdlCtx.PumpEvents(out var evt)) != GuiMessage.None)
            {
                Cap.OnEvent(true);

                if (msg == GuiMessage.Resize)
				    UpdateSize();
                else if (msg == GuiMessage.BackButton)
				    CurrentView.BackPressed();
                else if (msg == GuiMessage.KeyUp)
    				CurrentView.OnKeyPressed(evt.key.keysym);
                else if (msg == GuiMessage.Quit)
                    goto break_main_loop;

				ImGuiSDL2Impl.ProcessEvent(in evt);

#if ANDROID_LIB
                if (ImGui.GetIO().WantTextInput)
					sdlCtx.StartMobileTextInput();
				else if (!ImGui.GetIO().WantTextInput)
                    sdlCtx.StopMobileTextInput();
#endif
            }

            if (Cap.Cap())
                continue;

            ImGuiSDL2Impl.Renderer_NewFrame();
            ImGuiSDL2Impl.SDL2_NewFrame();
            ImGui.NewFrame();

            CurrentView.Draw();

            if (ShowDebugInfo)
                DebugWindow();

            ImGui.Render();

            CurrentView.RawDraw();

            ImGuiSDL2Impl.RenderDrawData(ImGui.GetDrawData());

            sdlCtx.Render();
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

        sdlCtx.DestroyWindow();
    }
}