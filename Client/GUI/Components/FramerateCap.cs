using ImGuiNET;
using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SysDVR.Client.GUI.Components
{
    public struct FramerateCapOptions
    {
        public static FramerateCapOptions Uncapped() => new() { Mode = CapMode.Uncapped };
        public static FramerateCapOptions Adaptive() => new() { Mode = CapMode.Adaptive };
        public static FramerateCapOptions Target(uint fps) => new() { Mode = CapMode.Target, DeltaCap = 1000u / fps };

        public enum CapMode
        {
            Target,
            Adaptive,
            Uncapped
        };

        public CapMode Mode;
        public uint DeltaCap;
    }

    internal class FramerateCap
    {
        // Override for debug
        public bool NeverCap = false;
        public FramerateCapOptions.CapMode CapMode => NeverCap ? FramerateCapOptions.CapMode.Uncapped : opt.Mode;

        FramerateCapOptions opt;
        int eventCounter;
        uint lastTick;

        public void SetMode(FramerateCapOptions mode)
        {
            opt = mode;

            if (opt.Mode == FramerateCapOptions.CapMode.Adaptive)
                OnEvent(true);
        }

        // Mark an event as received, needed for adaptive mode.
        // Thread safety: may be called by any thread
        public void OnEvent(bool important)
        {
            eventCounter = important ? 10 : 1;
        }

        // Called in the render loop, returns true if the frame should be skipped
        public bool Cap()
        {
            if (opt.Mode == FramerateCapOptions.CapMode.Uncapped || NeverCap)
                return false;

            if (opt.Mode == FramerateCapOptions.CapMode.Adaptive)
            {
                if (eventCounter > 0)
                {
                    eventCounter--;
                    return false;
                }

                SDL.SDL_Delay(20);
                return true;
            }

            if (opt.Mode == FramerateCapOptions.CapMode.Target)
            {
                var ticks = SDL.SDL_GetTicks();
                var delta = ticks - lastTick;
                if (delta < opt.DeltaCap)
                {
                    SDL.SDL_Delay(opt.DeltaCap - delta);
                    lastTick = SDL.SDL_GetTicks();
                }
                else lastTick = ticks;
            }

            return false;
        }
    }
}
