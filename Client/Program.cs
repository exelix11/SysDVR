using SDL2;
using SysDVR.Client.Core;
using SysDVR.Client.Platform;
using SysDVR.Client.Sources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SysDVR.Client
{
    public static class Program
    {
        public static ClientApp Instance;
        public static string Version = "6.0 ALPHA";

        public static Options Options = new();

#if ANDROID_LIB
	    [UnmanagedCallersOnly(EntryPoint = "sysdvr_entrypoint")]
	    public static void sysdvr_entrypoint(IntPtr __arg_init)
        {
            var init = NativeInitBlock.Read(__arg_init);
            Log.Setup(in init);
            
            RunApp(new string[0]);
        }
#else
        public static void Main(string[] args)
        {
            RunApp(args);
        }

        private static void Scan_OnDeviceFound(DeviceInfo obj)
        {
            Console.WriteLine(obj);
        }
#endif

        private static void RunApp(string[] args)
        {
#if !DEBUG
            try
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            }
            catch (Exception ex) 
            {
                Console.WriteLine("Failed to set global exception handler: " + ex.ToString());
            }

            try
            {
                TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            }
            catch (Exception ex) 
            {
                Console.WriteLine("Failed to set tasl exception handler: " + ex.ToString());
            }
#endif

#if !DEBUG
            try
#endif
            {
                Console.WriteLine("Starting SysDVR Client " + Version);

                if (Instance is null)
                {
                    DynamicLibraryLoader.Initialize();
                    Instance = new ClientApp();
                    Instance.Initialize();
                }

                Instance.EntryPoint(args);
            }
#if !DEBUG
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                SDL.SDL_Quit();
            }
#endif
        }

        private static void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            Console.WriteLine("TaskScheduler_UnobservedTaskException: " + e.Exception.ToString());
            SDL.SDL_Quit();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine("CurrentDomain_UnhandledException: " + e.ExceptionObject.ToString());
            SDL.SDL_Quit();
        }
    }
}