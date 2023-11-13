namespace SysDVR.Client;

using ImGuiNET;
using SDL2;
using SysDVR.Client.Core;
using SysDVR.Client.GUI;
using SysDVR.Client.GUI.Components;
using SysDVR.Client.Platform;
using SysDVR.Client.Targets.Player;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
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
		Console.WriteLine("Initializing app");

        LoadSettings();

		SDL_Init(SDL_INIT_VIDEO | SDL_INIT_AUDIO).AssertZero(SDL_GetError);

        var flags = SDL_image.IMG_InitFlags.IMG_INIT_JPG | SDL_image.IMG_InitFlags.IMG_INIT_PNG;
        SDL_image.IMG_Init(flags).AssertEqual((int)flags, SDL_image.IMG_GetError);

        SDL_SetHint(SDL_HINT_VIDEO_MINIMIZE_ON_FOCUS_LOSS, "0");

        SDL_SetHint(SDL_HINT_RENDER_SCALE_QUALITY, Program.Options.ScaleHintForSDL);
        
        ImGui.CreateContext();

        UnsafeImguiInitialization();
        ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
        ImGui.GetIO().NavVisible = true;

        // Fonts are loaded at double the resolution to reduce blurriness when scaling on high DPI
        FontText = ImGui.GetIO().Fonts.AddFontFromMemoryTTF(Resources.ReadResouce(Resources.MainFont), 30 * 2);
        FontH1 = ImGui.GetIO().Fonts.AddFontFromMemoryTTF(Resources.ReadResouce(Resources.MainFont), 45 * 2);
        FontH2 = ImGui.GetIO().Fonts.AddFontFromMemoryTTF(Resources.ReadResouce(Resources.MainFont), 40 * 2);

        BackupDeafaultStyle();
    }

    internal void EntryPoint(string[] args)
    {
        SDLThreadId = Thread.CurrentThread.ManagedThreadId;

        SdlWindow = SDL_CreateWindow("SysDVR-Client", SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED,
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

        UpdateSize();

        HandlePushView(new MainView());

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

//using System.Text;
//using System.Threading;
//using FFmpeg.AutoGen;
//using SysDVR.Client.Core;
//using SysDVR.Client.FileOutput;
//using SysDVR.Client.Sources;
//using SysDVR.Client.Targets.Player;
//using SysDVR.Client.Windows;

//namespace SysDVR.Client
//{
//    class Program
//    {
//        public static string BundledRuntimesFolder => Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location), "runtimes");

//        static string ArchName => RuntimeInformation.ProcessArchitecture switch
//        {
//            Architecture.X64 => "-x64",
//            Architecture.X86 => "-x86",
//            Architecture.Arm => "-arm",
//            Architecture.Arm64 => "-arm64",
//            _ => ""
//        };

//        static string OsName
//        {
//            get
//            {
//                if (OperatingSystem.IsWindows())
//                    return "win";
//                if (OperatingSystem.IsMacOS())
//                    return "mac";
//                // We don't currently support other OSes
//                else return "linux";
//            }
//        }

//        public static string BundledOsNativeFolder => Path.Combine(BundledRuntimesFolder, $"{OsName}{ArchName}", "native");

//        public static string? LibLoaderOverride = null;

//        public static string OsLibFolder
//        {
//            get
//            {
//                if (LibLoaderOverride is not null)
//                    return LibLoaderOverride;

//                if (OperatingSystem.IsWindows())
//                    return BundledOsNativeFolder;

//                // Should we really account for misconfigured end user PCs ? See https://apple.stackexchange.com/questions/40704/homebrew-installed-libraries-how-do-i-use-them
//                if (OperatingSystem.IsMacOS())
//                    return RuntimeInformation.ProcessArchitecture == Architecture.Arm64 ? "/opt/homebrew/lib/" : "/usr/local/lib/";

//                // On linux we have to rely on the dlopen implementation to find the libs wherever they are 
//                return string.Empty;
//            }
//        }

//        static IEnumerable<string> FindMacOSLibrary(string libraryName)
//        {
//            var names = new[] {
//                Path.Combine(OsLibFolder, $"lib{libraryName}.dylib"),
//                Path.Combine(OsLibFolder, $"{libraryName}.dylib"),
//                Path.Combine(BundledOsNativeFolder, $"lib{libraryName}.dylib"),
//                Path.Combine(BundledOsNativeFolder, $"{libraryName}.dylib"),
//                $"lib{libraryName}.dylib",
//                $"{libraryName}.dylib",
//                libraryName
//            };

//            return names;
//        }

//        static IntPtr MacOsLibraryLoader(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
//        {
//            IntPtr result = IntPtr.Zero;
//            foreach (var name in FindMacOSLibrary(libraryName))
//                if (NativeLibrary.TryLoad(name, out result))
//                    break;

//            if (result == IntPtr.Zero)
//                Console.Error.WriteLine($"Warning: couldn't load {libraryName} for {assembly.FullName} ({searchPath}).");

//            return result;
//        }

//        static void SetupMacOSLibrarySymlinks()
//        {
//            // This is a terrible hack but seems to work, we'll create symlinks to the OS libraries in the program folder
//            // The alternative is to fork libusbdotnet to add a way to load its native lib from a cusstom folder
//            // See https://github.com/exelix11/SysDVR/issues/192

//            var thisExePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

//            // We only need to link libusb as ffmpeg has a global root path variable and for SDL we set the custom NativeLoader callback
//            var libNames = new[] {
//                ("libusb-1.0", "libusb-1.0.dylib")
//            };

//            foreach (var (libName, fileName) in libNames)
//            {
//                if (File.Exists(Path.Combine(thisExePath, fileName)))
//                    continue;

//                var path = FindMacOSLibrary(libName).Where(File.Exists).FirstOrDefault();
//                if (string.IsNullOrWhiteSpace(path))
//                    Console.Error.WriteLine($"Couldn't find a library to symlink: {libName} ({fileName}). You might need to install it with brew.");
//                else
//                    File.CreateSymbolicLink(Path.Combine(thisExePath, fileName), path);
//            }
//        }

//        static void Main(string[] args)
//        {
//            try
//            {
//                new Program(args).ProgramMain();
//            }
//            catch (DllNotFoundException ex)
//            {
//                Console.Error.WriteLine($"There was an error loading a dynamic library. Make sure you installed all the dependencies and that you have the correct version of the libraries.");

//                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !Environment.Is64BitProcess)
//                {
//                    Console.ForegroundColor = ConsoleColor.Red;
//                    Console.Error.WriteLine("You are running the 32-bit version of .NET, on Windows this is NOT supported due to ffmpeg not providing official 32-bit versions of their libs.");

//                    if (Environment.Is64BitOperatingSystem)
//                    {
//                        Console.Error.WriteLine("Since you are using 64-bit Windows, uninstall x86 or 32-bit .NET and install the x64 version from Microsoft's website.");
//                    }
//                    else
//                    {
//                        Console.Error.WriteLine("It seems you're using 32-bit Windows, this is not supported and you should upgrade your PC.");
//                        Console.Error.WriteLine("However, you can try to make SysDVR work by downloading 32-bit ffmpeg builds and copying them to the SysDVR-client folder.");
//                    }

//                    Console.ResetColor();
//                }
//                else
//                {
//                    Console.Error.WriteLine(
//                        "If all libraries are properly installed ensure their path is set in your dynamic loader environment variable (PATH on Windows, LD_LIBRARY_PATH on Linux and DYLD_LIBRARY_PATH on MacOS)\r\n\r\n" +

//                        "In case of problems specific to ffmpeg or libavcodec you can override the loader path by adding to the command line --libdir <library path> where <library path> is the folder containing the dyamic libraries of ffmpeg (or symlinks to them).");

//                    if (OperatingSystem.IsLinux())
//                        Console.Error.WriteLine("For example on x64 linux you can try --libdir /lib/x86_64-linux-gnu/");
//                    else if (OperatingSystem.IsMacOS())
//                    {
//                        Console.Error.WriteLine("For example on MacOS you can try --libdir $(brew --prefix)/lib/");
//                        Console.ForegroundColor = ConsoleColor.Yellow;
//                        Console.Error.WriteLine("If you're using an arm based mac make sure you're using native arm dotnet and not the intel one.");
//                        Console.ResetColor();
//                    }
//                    else
//                        Console.Error.WriteLine("For example on Windows you can try --libdir C:\\Program Files\\ffmpeg\\bin");
//                }

//                Console.Error.WriteLine("\r\nFull error message:\r\n" + ex);
//            }
//            catch (Exception ex)
//            {
//                Console.Error.WriteLine("There was an error in SysDVR-Client.");
//                Console.Error.WriteLine("Make sure the SysDVR version on your console is the same as SysDVR-Client on your pc.");
//                Console.Error.WriteLine("After updating SysDVR on your console you should reboot.");

//                Console.Error.WriteLine("\r\nFull error message:");
//                Console.Error.WriteLine(ex);
//            }
//        }

//        string[] Args { get; init; }

//        bool HasArg(string arg) => Array.IndexOf(Args, arg) != -1;

//        string? ArgValue(string arg)
//        {
//            int index = Array.IndexOf(Args, arg);
//            if (index == -1) return null;
//            if (Args.Length <= index + 1) return null;

//            string value = Args[index + 1];
//            if (!value.Contains(' ') && value.StartsWith('"') && value.EndsWith('"'))
//                value = value.Substring(1, value.Length - 2);

//            return value;
//        }

//        int? ArgValueInt(string arg)
//        {
//            var a = ArgValue(arg);
//            if (int.TryParse(a, out int res))
//                return res;
//            return null;
//        }

//        Program(string[] args)
//        {
//            Args = args;
//        }

//        static string VersionString()
//        {
//            var Version = typeof(Program).Assembly.GetName().Version;
//            return Version is null ? "<unknown version>" :
//                $"SysDVR {Version.Major}.{Version.Minor}{(Version.Revision == 0 ? "" : $".{Version.Revision}")}";
//        }

//        void ProgramMain()
//        {
//            var StreamStdout = HasArg("--stdout");
//            var libOverride = ArgValue("--libdir");
//            DebugOptions.Current = DebugOptions.Parse(ArgValue("--debug"));

//            if (!DebugOptions.Current.NoProt)
//                AntiInject.Initialize();

//            // Native library loading memes
//            if (libOverride is not null)
//                LibLoaderOverride = libOverride;

//            ffmpeg.RootPath = OsLibFolder;

//            if (OperatingSystem.IsMacOS())
//            {
//                if (RuntimeInformation.OSArchitecture == Architecture.Arm64 && RuntimeInformation.ProcessArchitecture != Architecture.Arm64)
//                {
//                    Console.ForegroundColor = ConsoleColor.Red;
//                    Console.WriteLine("You're using the intel version of dotnet on an arm mac, this is not supported and will likely not work.");
//                    Console.WriteLine("Delete intel dotnet and install the native arm64 one.");
//                    Console.ResetColor();
//                }

//                NativeLibrary.SetDllImportResolver(typeof(SDL2.SDL).Assembly, MacOsLibraryLoader);
//                SetupMacOSLibrarySymlinks();
//            }

//            if (StreamStdout)
//                Console.SetOut(Console.Error);

//            Console.WriteLine($"SysDVR-Client - {VersionString()} by exelix");
//            Console.WriteLine("https://github.com/exelix11/SysDVR \r\n");

//            if (HandleStandaloneCommands())
//                return;

//            if (Args.Length == 0)
//                ShowShortGuide();

//            var streams = HandleStreamingCommands();

//            if (streams is not null)
//                StartStreaming(streams);
//        }

//        // Commands that don't do streaming (e.g. "help")
//        bool HandleStandaloneCommands()
//        {
//            if (Args.Length != 0 && Args[0].Contains("help"))
//                ShowFullGuide();
//            else if (HasArg("--version"))
//                return true;
//            else if (HasArg("--show-decoders"))
//                LibavUtils.PrintAllCodecs();
//            else if (HasArg("--debug-list"))
//                DebugOptions.PrintDebugOptionsHelp();
//            else
//                return false;

//            return true;
//        }

//        BaseStreamManager? HandleStreamingCommands()
//        {
//            bool StreamStdout = HasArg("--stdout");

//            BaseStreamManager StreamManager;
//            bool NoAudio, NoVideo;

//            NoAudio = HasArg("--no-audio");
//            NoVideo = HasArg("--no-video");

//            if (NoVideo && NoAudio)
//            {
//                Console.WriteLine("Specify at least a video or audio output");
//                return null;
//            }

//            // Stream destinations
//            if (StreamStdout)
//            {
//                if (!NoVideo && !NoAudio)
//                    NoAudio = true;
//                StreamManager = new StdOutManager(NoAudio ? StreamKind.Video : StreamKind.Audio);
//            }
//            else if (HasArg("--mpv"))
//            {
//                string mpvPath = ArgValue("--mpv");
//                if (mpvPath == null || !File.Exists(mpvPath))
//                {
//                    Console.WriteLine("The specified mpv path is not valid");
//                    return null;
//                }
//                if (!NoVideo && !NoAudio)
//                    NoAudio = true;
//                StreamManager = new MpvStdinManager(NoAudio ? StreamKind.Video : StreamKind.Audio, mpvPath);
//            }
//            else if (HasArg("--file"))
//            {
//                string filename = ArgValue("--file");
//                if (string.IsNullOrWhiteSpace(filename))
//                {
//                    Console.WriteLine("The specified path is not valid");
//                    return null;
//                }
//                if (!filename.EndsWith(".mp4", StringComparison.InvariantCultureIgnoreCase))
//                    Console.WriteLine($"Warning: {filename} doesn't end with .mp4, some programs may not be able to open it if you don't rename it manually.");
//                StreamManager = new Mp4OutputManager(filename, !NoVideo, !NoAudio);
//            }
//#if DEBUG
//            else if (HasArg("--record-debug"))
//            {
//                string path = ArgValue("--record-debug");
//                StreamManager = new LoggingManager(NoVideo ? null : Path.Combine(path, "video.h264"), NoAudio ? null : Path.Combine(path, "audio.raw"));
//            }
//#endif
//            else if (HasArg("--rtsp"))
//            {
//                int port = ArgValueInt("--rtsp-port") ?? 6666;
//                if (port <= 1024)
//                    Console.WriteLine("Warning: ports lower than 1024 are usually reserved and may require administrator/root privileges");
//                StreamManager = new RTSP.SysDvrRTSPManager(!NoVideo, !NoAudio, !HasArg("--rtsp-any-addr"), port);
//            }
//            else // Stream to the built-in player by default
//            {
//                StreamManager = new Player.PlayerManager(!NoVideo, !NoAudio, HasArg("--hw-acc"), ArgValue("--decoder"), ArgValue("--scale"))
//                {
//                    WindowTitle = ArgValue("--title"),
//                    StartFullScreen = HasArg("--fullscreen")
//                };
//            }

//            // Stream sources
//            if (Args.Length == 0 || Args[0] == "usb")
//            {
//                if (HasArg("--no-winusb"))
//                {
//                    Console.WriteLine("Note: the --no-winusb argument has been deprecated, it's now the default. You can remove it from the command line.");
//                }

//                var warnLevel = UsbContext.LogLevel.Error;

//                if (HasArg("--usb-warn")) warnLevel = UsbContext.LogLevel.Warning;
//                if (HasArg("--usb-debug")) warnLevel = UsbContext.LogLevel.Debug;

//                var ctx = FindUsbSource(warnLevel, ArgValue("--usb-serial"));
//                if (ctx == null)
//                    return null;

//                StreamManager.AddSource(new UsbStreamingSource(ctx, !NoVideo, !NoAudio));
//            }
//            else if (Args[0] == "bridge")
//            {
//                if (Args.Length < 2)
//                {
//                    Console.WriteLine("Specify an ip address for bridge mode");
//                    return null;
//                }

//                string ip = Args[1];

//                if (!NoVideo)
//                    StreamManager.AddSource(new TCPBridgeSource(ip, StreamKind.Video));
//                if (!NoAudio)
//                    StreamManager.AddSource(new TCPBridgeSource(ip, StreamKind.Audio));
//            }
//            // UI Test mode: only load libavcodec and SDL without streaming anything
//            else if (Args[0] == "stub")
//            {
//                StreamManager.AddSource(new StubSource(!NoVideo, !NoAudio));
//            }
//#if DEBUG
//            else if (Args[0] == "record")
//            {
//                var path = ArgValue("--source");

//                if (!NoVideo)
//                    StreamManager.AddSource(new RecordedSource(StreamKind.Video, path));
//                if (!NoAudio)
//                    StreamManager.AddSource(new RecordedSource(StreamKind.Audio, path));
//            }
//#endif
//            else
//            {
//                Console.WriteLine("Invalid source");
//                return null;
//            }

//            return StreamManager;
//        }

//        static UsbContext.SysDvrDevice? FindUsbSource(UsbContext.LogLevel usbLogLeve, string? preferredSerial)
//        {
//            var ctx = new UsbContext(usbLogLeve);

//            var devices = ctx.FindSysdvrDevices();

//            if (!string.IsNullOrWhiteSpace(preferredSerial))
//                preferredSerial = preferredSerial.ToLower().Trim();
//            else
//                preferredSerial = null;

//            if (devices.Count == 0)
//            {
//                Console.WriteLine("ERROR: SysDVR usb device not found.\r\n" +
//                    "Make sure that SysDVR is running in usb mode on your console and that you installed the correct driver.\r\n" +
//                    "SysDVR protocol may change with updates, SysDVR on your console must be the same version as the client !");

//                UsbDriverSuggest();
//                return null;
//            }
//            else if (devices.Count == 1)
//            {
//                if (preferredSerial is not null && devices[0].Serial.EndsWith(preferredSerial))
//                    Console.WriteLine($"Warning: Connecting to the console with serial {devices[0].Serial} instead of the requested {preferredSerial}");

//                Console.WriteLine($"Connecting to the console with serial {devices[0].Serial}...");
//                return devices[0];
//            }
//            else
//            {
//                var preferred = devices.Where(x => x.Serial.EndsWith(preferredSerial)).ToArray();
//                if (preferred.Length == 1)
//                {
//                    return preferred[0];
//                }
//                // Multiple partial matches ? look for the exact one
//                else if (preferred.Length >= 1)
//                {
//                    preferred = devices.Where(x => x.Serial == preferredSerial).ToArray();
//                    if (preferred.Length == 1)
//                    {
//                        return preferred[0];
//                    }
//                    else Console.WriteLine($"Warning: Multiple matches for {preferredSerial}, exact match not found");
//                }
//                else Console.WriteLine($"Warning: Requsted serial {preferredSerial} not found");

//                Console.WriteLine("Available SysDVR devices:");
//                for (int i = 0; i < devices.Count; i++)
//                    Console.WriteLine($"{i + 1}) {devices[i].Serial}");

//                Console.WriteLine("\r\nTIP: You can use the --usb-serial command line option to automatically select one based on the serial number");

//            select_value:
//                Console.Write("Enter the number of the device you want to use: ");
//                if (!int.TryParse(Console.ReadLine(), out int selection) || selection < 1 || selection > devices.Count)
//                {
//                    Console.WriteLine($"Error: expected value between 1 and {devices.Count}, try again");
//                    goto select_value;
//                }

//                return devices[selection - 1];
//            }
//        }

//        static void UsbDriverSuggest()
//        {
//            if (!OperatingSystem.IsWindows())
//                return;

//            Console.WriteLine();
//            Console.ForegroundColor = ConsoleColor.Yellow;
//            Console.WriteLine("Note that version 5.5 changed the required driver, if you installed the driver in the past you need to update it");
//            Console.Write("Open SysDVR-ClientGUI.exe and click on the install driver button. ");
//            Console.ResetColor();
//            Console.WriteLine();
//        }

//        void StartStreaming(BaseStreamManager streams)
//        {
//            streams.Begin();
//            bool terminating = false;

//            void Quit()
//            {
//                // this may be called at the same time by CTRL+C and main thread returning, dispose everything only once.
//                lock (this)
//                {
//                    if (terminating)
//                        return;
//                    terminating = true;
//                }

//                Console.WriteLine("Terminating threads...");
//                streams.Stop();
//                if (streams is IDisposable d)
//                    d.Dispose();
//                Environment.Exit(0);
//            }

//            Console.CancelKeyPress += delegate (object instance, ConsoleCancelEventArgs args)
//            {
//                args.Cancel = true;
//                Quit();
//            };

//            streams.MainThread();

//            Quit();
//        }

//        void ShowShortGuide()
//        {
//            Console.WriteLine("Basic usage:\r\n" +
//                        "Simply launching this exectuable will show this message and launch the video player via USB.\r\n" +
//                        "Use 'SysDVR-Client usb' to stream directly, add '--no-audio' or '--no-video' to disable one of the streams\r\n" +
//                        "To stream in TCP Bridge mode launch 'SysDVR-Client bridge <switch ip address>'\r\n" +
//                        "There are more advanced options, you can see them with 'SysDVR-Client --help'\r\n" +
//                        "Press enter to continue.\r\n");
//            Console.ReadLine();
//        }

//        void ShowFullGuide()
//        {
//            Console.WriteLine(
//@"Usage:
//SysDVR-Client.exe <Stream source> [Source options] [Stream options] [Output options]

//Stream sources:
//	The source mode is how the client connects to SysDVR running on the console. Make sure to set the correct mode with SysDVR-Settings.
//	`usb` : Connects to SysDVR via USB, used if no source is specified. Remember to setup the driver as explained on the guide
//	`bridge <IP address>` : Connects to SysDVR via network at the specified IP address, requires a strong connection between the PC and switch (LAN or full signal wireless)
//	Note that the `Simple network mode` option in SysDVR-Settings does not require the client, you must open it directly in a video player.

//Source options:
//	`--usb-warn` : Enables printing warnings from the usb stack, use it to debug USB issues
//	`--usb-debug` : Same as `--usb-warn` but more detailed
//	`--usb-serial NX0000000` : When multiple consoles are plugged in via USB use this option to automatically select one by serial number. 
//		This also matches partial serials starting from the end, for example NX012345 will be matched by doing --usb-serial 45

//Stream options:
//	`--no-video` : Disable video streaming, only streams audio
//	`--no-audio` : Disable audio streaming, only streams video

//Output options:
//	If you don't specify any option the built-in video player will be used.
//	Built-in player options:
//	`--hw-acc` : Try to use hardware acceleration for decoding, this option uses the first detected decoder, it's recommended to manually specify the decoder name with --decoder
//	`--decoder <name>` : Use a specific decoder for ffmpeg decoding, you can see all supported codecs with --show-decoders
//	`--scale <quality>` : Use a specific quality for scaling, possible values are `nearest`, `linear` and `best`. `best` may not be available on all PCs, see SDL docs for SDL_HINT_RENDER_SCALE_QUALITY, `linear` is the default mode.
//	`--fullscreen` : Start in full screen mode. Press F11 to toggle manually
//	`--title <some text>` : Adds the argument string to the title of the player window

//	RTSP options:
//	`--rtsp` : Relay the video feed via RTSP. SysDVR-Client will act as an RTSP server, you can connect to it with RTSP with any compatible video player like mpv or vlc
//	`--rtsp-port <port number>` : Port used to stream via RTSP (default is 6666)
//	`--rtsp-any-addr` : By default only the pc running SysDVR-Client can connect to the RTSP stream, enable this to allow connections from other devices in your local network

//	Low-latency streaming options:
//	`--mpv <mpv path>` : Streams the specified channel to mpv via stdin, only works with one channel, if no stream option is specified `--no-audio` will be used.
//	`--stdout` : Streams the specified channel to stdout, only works with one channel, if no stream option is specified `--no-audio` will be used.

//	Storage options
//	`--file <output path>` : Saves an mp4 file to the specified folder, existing files will be overwritten.	

//Extra options:
//	These options will not stream, they just print the output and then quit.
//	`--show-decoders` : Prints all video codecs available for the built-in video player
//	`--version` : Prints the version
//	`--libdir` : Overrides the dynami library loading path, use only if dotnet can't locate your ffmpeg/avcoded/SDL2 libraries automatically
//                 this option effect depend on your platform, some libraries may still be handled by dotnet, it's better to use your loader path environment variable.
//	`--debug <debug options>` : Enables debug options. Multiple options are comma separated for example: --debug log,stats
//				When a debugger is attached `log` is enabled by default.
//	`--debug-list` : Prints all available debug options.

//Command examples:
//	SysDVR-Client.exe usb
//		Connects to switch via USB and streams video and audio in the built-in player

//	SysDVR-Client.exe usb --rtsp	
//		Connects to switch via USB and streams video and audio via rtsp at rtsp://127.0.0.1:6666/

//	SysDVR-Client.exe bridge 192.168.1.20 --no-video --rtsp-port 9090
//		Connects to switch via network at 192.168.1.20 and streams the audio over rtsp at rtsp://127.0.0.1:9090/

//	SysDVR-Client.exe usb --mpv `C:\Program Files\mpv\mpv.com`
//		Connects to switch via USB and streams the video in low-latency mode via mpv
//");
//        }
//    }
//}
