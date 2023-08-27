using SysDVR.Client.Core;
using SysDVR.Client.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SysDVR.Client
{
    public static class Program
    {
        public static ClientApp Instance;
        public static string Version;

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
#endif
        
        private static void RunApp(string[] args)
        {
            try
            {
                {
                    var ver = typeof(Program).Assembly.GetName().Version;

                    Version = ver is null ? "<unknown version>" :
                        $"{ver.Major}.{ver.Minor}{(ver.Revision == 0 ? "" : $".{ver.Revision}")}";
                }

                DynamicLibraryLoader.Initialize();
                Instance = new ClientApp();
                Instance.EntryPoint(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace.ToString());
                Console.WriteLine(ex.InnerException.ToString());
                Console.WriteLine(ex.InnerException?.StackTrace?.ToString());
            }
        }
    }
}