using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SysDVR.Client.GUI
{
    public abstract class View
    {
        public bool UsesImgui = true;
        public FramerateCapOptions RenderMode = FramerateCapOptions.Target(30);

        public abstract void Draw();

        public virtual void ResolutionChanged() { }

        public virtual void RawDraw()
        {
            Program.Instance.ClearScrren();
        }

        public virtual void BackPressed()
        {
            Program.Instance.PopView();
        }

        public virtual void EnterForeground() 
        {
            ResolutionChanged();
        }

        public virtual void LeveForeground() { }
        public virtual void Destroy() { }
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

        public static void BeginWindow(string name)
        {
            ImGui.SetNextWindowSize(ImGui.GetIO().DisplaySize);
            ImGui.SetNextWindowPos(Vector2.Zero);
            ImGui.Begin(name, ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoBringToFrontOnFocus);
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
            var size = ImGui.CalcTextSize(text);
            var pos = ImGui.GetCursorPos();
            ImGui.SetCursorPos(new Vector2((ImGui.GetContentRegionAvail().X - size.X) / 2, pos.Y));
            ImGui.Text(text);
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
