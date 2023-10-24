using SysDVRClientGUI.Forms;
using SysDVRClientGUI.Forms.DriverInstall;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysDVRClientGUI
{
	static class Program
	{
		/// <summary>
		/// Punto di ingresso principale dell'applicazione.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
            // When build with dotnet instead of msbuld including binary resources like the icon in form
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

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form mainForm;

            // Used when self-launching elevated
            if (args.Length > 0 && args[0] == "--install-driver")
            {
                mainForm = new DriverInstallForm(true);
            }
			else
			{
				mainForm = new Main();
			}

			Application.Run(mainForm);
		}
        
		public static Icon ApplicationIcon = null;

		public static string RuntimesFolder => Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location), "runtimes");

		public static string OsArchGenericFolder => Path.Combine(RuntimesFolder, "win");

		public static string OsNativeFolder => Path.Combine(RuntimesFolder, $"win-{(Environment.Is64BitProcess ? "x64" : "x86")}", "native");
	}
}
