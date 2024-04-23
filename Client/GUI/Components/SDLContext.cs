using ImGuiNET;
using SDL2;
using SysDVR.Client.Core;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using static SDL2.SDL;

namespace SysDVR.Client.GUI.Components
{
	public enum GuiMessage
	{
		None,
		Other,
		Quit,
		Resize,
		KeyUp,
		KeyDown,
		FullScreen,
		BackButton
	}

	public class SDLContext
	{
		// Detect bugs such as multiple view pushes in a row
		int SDLThreadId;

		internal IntPtr WindowHandle { get; private set; }
		internal IntPtr RendererHandle { get; private set; }

		public bool IsFullscreen { get; private set; } = false;
		public Vector2 WindowSize { get; private set; }

		// Scaling info
		Vector2 PixelSize;
		Vector2 WantedDPIScale;
		bool IsWantedScale;

		// On android we must manually check for when imgui needs the keyboard and open it
		// TODO: Will this also open the keyboard when there's a physical one connected?
#if ANDROID_LIB
		bool usingTextinput;
#endif

		// SDL functions must only be called from its main thread
		// but it may not always trigger an exception
		// force it to crash so we know there's a bug
		public void BugCheckThreadId()
		{
			if (SDLThreadId != Thread.CurrentThread.ManagedThreadId)
				throw new InvalidOperationException($"SDL Bug check on thread {Thread.CurrentThread.ManagedThreadId} insteadl of {SDLThreadId}.");
		}

		public void SetNewThreadOwner()
		{
			SDLThreadId = Thread.CurrentThread.ManagedThreadId;
		}

		public SDLContext()
		{
			Program.DebugLog("Initializing SDL");

			SDL_Init(SDL_INIT_VIDEO | SDL_INIT_AUDIO | SDL_INIT_JOYSTICK).AssertZero(SDL_GetError);

			var flags = SDL_image.IMG_InitFlags.IMG_INIT_JPG | SDL_image.IMG_InitFlags.IMG_INIT_PNG;
			SDL_image.IMG_Init(flags).AssertEqual((int)flags, SDL_image.IMG_GetError);

			SDL_SetHint(SDL_HINT_VIDEO_MINIMIZE_ON_FOCUS_LOSS, "0");

			SDL_SetHint(SDL_HINT_RENDER_SCALE_QUALITY, Program.Options.ScaleHintForSDL);

			SDLThreadId = Thread.CurrentThread.ManagedThreadId;
		}

		public void CreateWindow(string? windowTitle)
		{
			windowTitle = windowTitle is null ?
				"SysDVR-Client" : $"{windowTitle} - SysDVR-Client";

			BugCheckThreadId();
			DestroyWindow();

			WindowHandle = SDL_CreateWindow(windowTitle, SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED,
				StreamInfo.VideoWidth, StreamInfo.VideoHeight,
				SDL_WindowFlags.SDL_WINDOW_ALLOW_HIGHDPI | SDL_WindowFlags.SDL_WINDOW_RESIZABLE)
				.AssertNotNull(SDL_GetError);

			var flags = Program.Options.ForceSoftwareRenderer ? SDL_RendererFlags.SDL_RENDERER_SOFTWARE :
				(SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);

			RendererHandle = SDL_CreateRenderer(WindowHandle, -1, flags).AssertNotNull(SDL_GetError);

			SDL_GetRendererInfo(RendererHandle, out var info);
			Console.WriteLine($"Initialized SDL with {Marshal.PtrToStringAnsi(info.name)} renderer");

			UpdateSize();
		}

		private bool UpdateSize()
		{
			var cur = WindowSize;
			SDL_GetWindowSize(WindowHandle, out int w, out int h);

			if (cur == new Vector2(w, h))
				return false;

			WindowSize = new(w, h);

			// Scaling workaround for OSX, SDL_WINDOW_ALLOW_HIGHDPI doesn't seem to work
			//if (OperatingSystem.IsMacOS())
			{
				SDL_GetRendererOutputSize(RendererHandle, out int pixelWidth, out int pixelHeight);
				PixelSize = new(pixelWidth, pixelHeight);

				float dpiScaleX = pixelWidth / (float)w;
				float spiScaleY = pixelHeight / (float)h;
				WantedDPIScale = new(dpiScaleX, spiScaleY);

				IsWantedScale = SDL_RenderSetScale(RendererHandle, dpiScaleX, spiScaleY) == 0;
			}

			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public GuiMessage PumpEvents(out SDL_Event evt)
		{
			if (SDL_PollEvent(out evt) == 0)
				return GuiMessage.None;

			//Console.WriteLine($"Received SDL_Event {evt.type}");

#if ANDROID_LIB
			// Fix for Samsung Dex software touchpad inputs
			if (evt.type is SDL_EventType.SDL_MOUSEBUTTONDOWN or SDL_EventType.SDL_MOUSEBUTTONUP)
			{
				if (evt.button.button == 0)
					evt.button.button = 1;
			}
#endif

			if (evt.type == SDL_EventType.SDL_QUIT ||
					(evt.type == SDL_EventType.SDL_WINDOWEVENT && evt.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_CLOSE))
			{
				return GuiMessage.Quit;
			}
			else if (evt.type == SDL_EventType.SDL_WINDOWEVENT && evt.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED)
			{
				return UpdateSize() ? GuiMessage.Resize : GuiMessage.Other;
			}
			else if (evt.type == SDL_EventType.SDL_KEYDOWN && evt.key.keysym.sym == SDL_Keycode.SDLK_F11)
			{
				SetFullScreen(!IsFullscreen);
				return GuiMessage.FullScreen;
			}
			else if (evt.type == SDL_EventType.SDL_KEYDOWN && evt.key.keysym.sym is SDL_Keycode.SDLK_ESCAPE or SDL_Keycode.SDLK_AC_BACK)
			{
				return GuiMessage.BackButton;
			}
			else if (evt.type == SDL_EventType.SDL_KEYDOWN)
			{
				return GuiMessage.KeyDown;
			}
			else if (evt.type == SDL_EventType.SDL_KEYUP)
			{
				// At least on Windows keeping a key pressed spams of keydown and textinput events due to the text input behavior
				// This also affects imgui IO keydown events.
				// The only event that is guaranteed to fire is the keyup event so we use it to determine when a key has been pressed and then released
				return GuiMessage.KeyUp;
			}
			else if (evt.type == SDL_EventType.SDL_RENDER_DEVICE_RESET)
			{
				// This should not happen on modern android versions according to SDL docs
				Console.WriteLine("SDL failed to resume, terminating.");
				return GuiMessage.Quit;
			}
			else if (evt.type == SDL_EventType.SDL_JOYBUTTONDOWN && evt.jbutton.button == 1) // SDL_CONTROLLER_BUTTON_B
			{
				return GuiMessage.BackButton;
			}

			return GuiMessage.Other;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Render()
		{
			SDL_RenderPresent(RendererHandle);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void StartMobileTextInput()
		{
#if !ANDROID_LIB
			throw new Exception("This method is only valid on android");
#else
			if (usingTextinput)
				return;

			SDL.SDL_StartTextInput();
			usingTextinput = true;
#endif
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void StopMobileTextInput()
		{
#if !ANDROID_LIB
			throw new Exception("This method is only valid on android");
#else
			if (!usingTextinput)
				return;

			SDL.SDL_StopTextInput();
			usingTextinput = false;
#endif
		}

		public void DestroyWindow()
		{
			if (RendererHandle != IntPtr.Zero)
			{
				SDL_DestroyRenderer(RendererHandle);
				RendererHandle = IntPtr.Zero;
			}

			if (WindowHandle != IntPtr.Zero)
			{
				SDL_DestroyWindow(WindowHandle);
				WindowHandle = IntPtr.Zero;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ClearScreen()
		{
			SDL_SetRenderDrawColor(RendererHandle, 0x0, 0x0, 0x0, 0xFF);
			SDL_RenderClear(RendererHandle);
		}

		public void SetFullScreen(bool enableFullScreen)
		{
			IsFullscreen = enableFullScreen;
			SDL_SetWindowFullscreen(WindowHandle, enableFullScreen ? (uint)SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP : 0);
			SDL_ShowCursor(enableFullScreen ? SDL_DISABLE : SDL_ENABLE);
		}

		public string GetDebugInfo()
		{
			StringBuilder sb = new();
			sb.AppendLine($"SDLThreadId: {SDLThreadId} Window Size: {WindowSize}");
			sb.AppendLine($"Pixel Size: {PixelSize} DPI Scale: {WantedDPIScale} ({IsWantedScale})");
			return sb.ToString();
		}
	}
}
