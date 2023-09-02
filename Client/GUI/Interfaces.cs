using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace SysDVR.Client.GUI
{
    public abstract class View
    {
        public FramerateCapOptions RenderMode = FramerateCapOptions.Target(30);

        public abstract void Draw();

        public virtual void ResolutionChanged() { }

        public virtual void DrawDebug() { }

        public virtual void RawDraw()
        {
            Program.Instance.ClearScrren();
        }

        public virtual void BackPressed()
        {
            Program.Instance.PopView();
        }

        public virtual void Created() { }
        public virtual void EnterForeground() { }
        public virtual void Destroy() { }
        public virtual void LeaveForeground() { }
    }

    public static class Gui 
    {
        public struct CenterGroup 
        {
            private float X;

            public void Reset() 
            {
                X = 0;
            }

            public void StartHere() 
            {
                ImGui.SetCursorPosX(X);
            }

            public void EndHere() 
            {
                ImGui.SameLine();
                var w = ImGui.GetWindowSize().X;
                var len = ImGui.GetCursorPosX() - X;
                X = w / 2 - len / 2;
                ImGui.NewLine();
            }
        }

        // https://github.com/ocornut/imgui/issues/3379
        public static void MakeWindowScrollable()
        {
            var rect = new ImRect()
            {
                Min = ImGui.GetWindowPos(),
                Max = ImGui.GetWindowPos() + ImGui.GetWindowSize(),
            };

            var id = ImGui.GetID("##ScrollOverlay");
            ImGui.KeepAliveID(id);
            ImGui.ButtonBehavior(rect, id, out _, out var held, ImGuiButtonFlags.MouseButtonLeft);

            if (held)
            {
                ImGui.SetScrollY(ImGui.GetScrollY() - ImGui.GetIO().MouseDelta.Y);
            }
        }

        public class Popup 
        {
            public readonly string Name;
            public bool IsOpen { get; private set; }

            bool shouldOpen;

            public Popup(string name)
            {
                Name = name;
            }

            public void RequestOpen() 
            {
                shouldOpen = true;
            }

            public void RequestClose() 
            {
                IsOpen = false;
            }

            public bool Begin()
            {
                return Begin(Vector2.Zero);
            }

            public bool Begin(Vector2 size) 
            {
                if (shouldOpen)
                {
                    if (!IsOpen)
                    {
                        ImGui.OpenPopup(Name);
                        IsOpen = true;
                        shouldOpen = false;
                    }
                }

                if (!IsOpen)
                    return false;

                ImGuiWindowFlags flags = ImGuiWindowFlags.NoMove;
                if (size == Vector2.Zero)
                {
                    ImGui.SetNextWindowPos(ImGui.GetIO().DisplaySize / 2, ImGuiCond.Appearing, new(0.5f, 0.5f));
                    flags |= ImGuiWindowFlags.AlwaysAutoResize;
                }
                else
                {
                    ImGui.SetNextWindowSize(size);
                    ImGui.SetNextWindowPos(ImGui.GetIO().DisplaySize / 2 - size / 2);
                    flags |= ImGuiWindowFlags.NoResize;
                }
                return ImGui.BeginPopupModal(Name, flags);
            }

            // true: steals the button
            public bool HandleBackButton() 
            {
                if (IsOpen)
                {
                    RequestClose();
                    return true;
                }

                return false;
            }
        }

        public static void EndWindow() 
        {
            MakeWindowScrollable();
            ImGui.End();
        }

        public static void BeginWindow(string name, ImGuiWindowFlags extraFlags = ImGuiWindowFlags.None)
        {
            ImGui.SetNextWindowSize(ImGui.GetIO().DisplaySize);
            ImGui.SetNextWindowPos(Vector2.Zero);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0);
            ImGui.Begin(name, 
                ((ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoMove | 
                ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoBringToFrontOnFocus) &
                ~ImGuiWindowFlags.NoScrollbar) | extraFlags);
            ImGui.PopStyleVar();
        }

        public static void CenterImage(Image image, int height)
        {
            var width = (int)(image.Width * ((float)height / image.Height));
            var pos = ImGui.GetCursorPos();
            ImGui.SetCursorPos(new Vector2((ImGui.GetWindowSize().X - width) / 2, pos.Y));
            ImGui.Image(image.Texture, new Vector2(width, height));            
        }

        public static void CenterText(string text)
        {
            var size = ImGui.CalcTextSize(text, false, ImGui.GetContentRegionAvail().X);
            var pos = ImGui.GetCursorPos();
            ImGui.SetCursorPos(new Vector2((ImGui.GetContentRegionAvail().X - size.X) / 2, pos.Y));
            ImGui.TextWrapped(text);
        }

        public static bool CenterButton(string text)
        {
            var size = ImGui.CalcTextSize(text) + new Vector2(ImGui.GetStyle().FramePadding.X * 2, ImGui.GetStyle().FramePadding.X * 2);
            return CenterButton(text, size);
        }

        public static bool CenterButton(string text, Vector2 size)
        {
            var pos = ImGui.GetCursorPos();
            ImGui.SetCursorPos(new Vector2((ImGui.GetWindowSize().X - size.X) / 2, pos.Y));
            return ImGui.Button(text, size);
        }

        public static float ButtonHeight() 
        {
            var styley = ImGui.GetStyle().WindowPadding.Y;
            return ImGui.CalcTextSize("AAA").Y + styley * 2;
        }

        public static void CursorFromBottom(float height)
        {
            var styley = ImGui.GetStyle().WindowPadding.Y;
            var y = ImGui.GetWindowSize().Y;
            var cur = ImGui.GetCursorPosY();

            y -= styley + height;

            // If the layout is already too low do nothing to avoid overlapping components
            if (cur < y)
                ImGui.SetCursorPosY(y);
        }

        public static void H1() 
        {
            ImGui.PushFont(Program.Instance.FontH1);
        }

        public static void H2()
        {
            ImGui.PushFont(Program.Instance.FontH2);
        }
    }
}
