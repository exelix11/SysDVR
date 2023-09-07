﻿using SDL2;
using SysDVR.Client.Core;
using SysDVR.Client.GUI;
using SysDVR.Client.Targets.Player;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SysDVR.Client.Platform
{
    internal static class Resources
    {
#if ANDROID_LIB
        // Android is such a shit platform that you can't fopen(), all resources must be read like this
        // thank god SDL already wraps its stupid interface
        public static byte[] ReadResouce(string path)
        {
            Console.WriteLine($"Loading resource {path}");

            var file = SDL.SDL_RWFromFile(path, "r").AssertNotNull(SDL.SDL_GetError);

            var len = (int)SDL.SDL_RWsize(file);
            var buf = new byte[len];

            var read = SDL.SDL_RWread(file, buf, 1, len);
            if (read != len)
                throw new Exception($"Loading resource {path} failed: {SDL.SDL_GetError()}");

            SDL.SDL_RWclose(file);
            return buf;
        }


        static string BasePath = "";        
        static string ResourcePath(string x) => x;
#else
        static string BasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "runtimes");
        
        static string ResourcePath(string x) => Path.Combine(BasePath, "resources", x);
        
        public static byte[] ReadResouce(string path) => File.ReadAllBytes(path);
#endif

        public static string RuntimesFolder => BasePath;
        public static string MainFont => ResourcePath("OpenSans.ttf");
        public static string LoadingImage => ResourcePath("loading.yuv");

        public readonly static LazyImage Logo = new LazyImage(ResourcePath("logo.png"));
        public readonly static LazyImage UsbIcon = new LazyImage(ResourcePath("ico_usb.png"));
        public readonly static LazyImage WifiIcon = new LazyImage(ResourcePath("ico_wifi.png"));

        public static Stream OpenFileForRecording()
        {
            var path = Path.Combine(Program.Options.RecordingsPath, $"SysDVR_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.np4");
            return File.OpenWrite(path);
        }

        public static string? GetBuildId() 
        {
            try
            {
                var s = ReadResouce(ResourcePath("buildid.txt"));
                return Encoding.UTF8.GetString(s);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Couldn't load build id file" + ex.ToString());
            }

            return null;
        }
    }
}