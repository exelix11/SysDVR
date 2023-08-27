using ImGuiNET;
using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysDVR.Client.GUI
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
                OnEvent(); 
        }

        // Mark an event as received, needed for adaptive mode
        public void OnEvent() 
        {
            eventCounter = 10;
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
                
                SDL.SDL_Delay(30);
                return true;
            }

            if (opt.Mode == FramerateCapOptions.CapMode.Target)
            {
                var ticks = SDL.SDL_GetTicks();
                var delta = ticks - lastTick;
                lastTick = ticks;
                if (delta < opt.DeltaCap)
                {
                    SDL.SDL_Delay(opt.DeltaCap - delta);
                }
            }

            return false;
        }
    }
}
