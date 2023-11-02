using Serilog;
using Serilog.Events;
using SysDVRClientGUI.Forms;
using SysDVRClientGUI.Forms.DriverInstall;
using SysDVRClientGUI.Logic;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using static SysDVRClientGUI.Logic.Constants;

namespace SysDVRClientGUI
{
    static class Program
    {
#if DEBUG
        public readonly static LogEventLevel level = LogEventLevel.Verbose;
#else
        public readonly static LogEventLevel level = LogEventLevel.Information;
#endif
        public static Icon ApplicationIcon { get; private set; }
        /// <summary>
        /// Punto di ingresso principale dell'applicazione.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // When build with dotnet instead of msbuild including binary resources like the icon in form
            // resources requires an extra dependency that is only available from netfx 4.6+ and we target 4.5,
            // since we only need the icon we can just get it from the main executable instead.
            try
            {
                ApplicationIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            }
            catch
            {
                // Doesn't really matter if this fails
            }

            Assembly assembly = typeof(Program).Assembly;
            string appPath = Path.GetDirectoryName(typeof(Program).Assembly.Location);
            string logFileSuffix = null;
#if DEBUG
            logFileSuffix = "_debug";
#endif

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("application", assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title)
                .Enrich.WithProperty("version", assembly.GetName().Version.ToString())
                .MinimumLevel.Verbose()
                .WriteTo.Debug(restrictedToMinimumLevel: level)
                .WriteTo.File($"{Path.Combine(appPath, "log", $"clientgui{logFileSuffix}.log")}", rollOnFileSizeLimit: true, fileSizeLimitBytes: 1024000, restrictedToMinimumLevel: level)
                .CreateLogger();

            Log.Verbose("Logger initialized");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form mainForm;

            // Used when self-launching elevated
            if (args.Length > 0 && args[0] == "--install-driver")
            {
                mainForm = new DriverInstallForm(true);
                Log.Information("Appliction start in \"--install-driver\" mode.");
            }
            else
            {
                Log.Information("Normal application start");
                RuntimeStorage.Config = new(Path.Combine(appPath, USERCONFIG_FILE));
                RuntimeStorage.Config.Load();
                mainForm = new Main();
            }

            Application.Run(mainForm);
        }

        public static string RuntimesFolder => Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location), "runtimes");

        public static string OsArchGenericFolder => Path.Combine(RuntimesFolder, "win");

        public static string OsNativeFolder => Path.Combine(RuntimesFolder, $"win-{(Environment.Is64BitProcess ? "x64" : "x86")}", "native");
    }
}
