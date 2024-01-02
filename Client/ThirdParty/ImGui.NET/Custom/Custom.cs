using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;

namespace ImGuiNET
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ImRect
    {
        public Vector2 Min;
        public Vector2 Max;
    };

    public enum ImGuiItemFlags : uint
    {
        // Controlled by user
        None = 0,
        NoTabStop = 1 << 0,  // false     // Disable keyboard tabbing. This is a "lighter" version of ImGuiItemFlags_NoNav.
        ButtonRepeat = 1 << 1,  // false     // Button() will return true multiple times based on io.KeyRepeatDelay and io.KeyRepeatRate settings.
        Disabled = 1 << 2,  // false     // Disable interactions but doesn't affect visuals. See BeginDisabled()/EndDisabled(). See github.com/ocornut/imgui/issues/211
        NoNav = 1 << 3,  // false     // Disable any form of focusing (keyboard/gamepad directional navigation and SetKeyboardFocusHere() calls)
        NoNavDefaultFocus = 1 << 4,  // false     // Disable item being a candidate for default focus (e.g. used by title bar items)
        SelectableDontClosePopup = 1 << 5,  // false     // Disable MenuItem/Selectable() automatically closing their popup window
        MixedValue = 1 << 6,  // false     // [BETA] Represent a mixed/indeterminate value, generally multi-selection where values differ. Currently only supported by Checkbox() (later should support all sorts of widgets)
        ReadOnly = 1 << 7,  // false     // [ALPHA] Allow hovering interactions but underlying value is not changed.
        NoWindowHoverableCheck = 1 << 8,  // false     // Disable hoverable check in ItemHoverable()
        AllowOverlap = 1 << 9,  // false     // Allow being overlapped by another widget. Not-hovered to Hovered transition deferred by a frame.

        // Controlled by widget code
        Inputable = 1 << 10, // false     // [WIP] Auto-activate input mode when tab focused. Currently only used and supported by a few items before it becomes a generic feature.
    };

    public static unsafe partial class ImGui
    {
        [DllImport("cimgui", EntryPoint = "igButtonBehavior", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool ButtonBehavior(ImRect bb, uint id, out bool out_hovered, out bool out_held, ImGuiButtonFlags flags);

        [DllImport("cimgui", EntryPoint = "igRenderNavHighlight", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RenderNavHighlight(ImRect bb, uint id, int flags);

        [DllImport("cimgui", EntryPoint = "igRenderFrame", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RenderFrame(Vector2 p_min, Vector2 p_max, uint fill_col, bool border, float rounding);

        [DllImport("cimgui", EntryPoint = "igItemAdd", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool ItemAdd(ImRect bb, uint id, IntPtr nav_bb, ImGuiItemFlags extra_flags);

        [DllImport("cimgui", EntryPoint = "igKeepAliveID", CallingConvention = CallingConvention.Cdecl)]
        public static extern void KeepAliveID(uint id);
    }

    public static class ImGuiSDL2Impl {
        [DllImport("cimgui", EntryPoint = "ImGui_ImplSDL2_InitForSDLRenderer")]
        public static extern bool InitForSDLRenderer(nint window, nint renderer);

        [DllImport("cimgui", EntryPoint = "ImGui_ImplSDLRenderer2_Init")]
        public static extern bool Init(nint renderer);

        [DllImport("cimgui", EntryPoint = "ImGui_ImplSDL2_ProcessEvent")]
        public static extern bool ProcessEvent(in SDL_Event evt);

        [DllImport("cimgui", EntryPoint = "ImGui_ImplSDLRenderer2_NewFrame")]
        public static extern void Renderer_NewFrame();

        [DllImport("cimgui", EntryPoint = "ImGui_ImplSDL2_NewFrame")]
        public static extern void SDL2_NewFrame();

        [DllImport("cimgui", EntryPoint = "ImGui_ImplSDLRenderer2_RenderDrawData")]
        public static extern void RenderDrawData(ImDrawDataPtr ptr);

        [DllImport("cimgui", EntryPoint = "ImGui_ImplSDLRenderer2_Shutdown")]
        public static extern void Renderer_Shutdown();

        [DllImport("cimgui", EntryPoint = "ImGui_ImplSDL2_Shutdown")]
        public static extern void SDL2_Shutdown();
    }
}
