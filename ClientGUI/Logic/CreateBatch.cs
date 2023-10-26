using SysDVRClientGUI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysDVRClientGUI.Logic
{
    internal class CreateBatch
    {
        const string BatchLauncherFileCheckTemplate =
@":: Ensure {1} file exists
if not exist ""{0}"" (
    echo.
    echo Could not find {1}, create a new launcher from the GUI.
    pause
    exit /b 1
)

";
        private string batchBuffer;
        public string ClientDllPath { get; init; }
        public IEnumerable<LaunchCommand> LaunchCommands { get; init; }

        #region Constructor
        public CreateBatch(string clientDllPath, IEnumerable<LaunchCommand> launchCommands)
        {
            if (string.IsNullOrEmpty(clientDllPath) || !launchCommands.Any())
            {
                throw new ArgumentException("Parameters cannot be null or empty");
            }

            this.ClientDllPath = clientDllPath;
            this.LaunchCommands = launchCommands;
        }
        #endregion

        public void Create()
        {
            StringBuilder bat = new();

            bat.AppendLine("@echo off");
            bat.AppendLine("title SysDVR Launcher");
            bat.AppendLine("echo Launching SysDVR-Client...");

            // Check all dependencies first
            foreach (var cmd in this.LaunchCommands)
            {
                bat.AppendFormat(BatchLauncherFileCheckTemplate, cmd.Executable, Path.GetFileName(cmd.Executable));

                if (cmd.FileDependencies != null)
                    foreach (var dep in cmd.FileDependencies)
                        bat.AppendFormat(BatchLauncherFileCheckTemplate, dep, Path.GetFileName(dep));
            }

            // cd to sysdvr folder so that the dll can be found
            bat.AppendLine($"cd /D \"{Path.GetDirectoryName(Path.GetFullPath(this.ClientDllPath))}\"");

            // If there are multiple commands use start to launch them in parallel
            string prefix = this.LaunchCommands.Count() > 1 ? "start " : "";

            // Launch the commands
            foreach (var cmd in this.LaunchCommands)
            {
                bat.AppendLine($"{prefix}{cmd}");
                // If there are multiple commands wait a bit before launching the next one

                if (!string.IsNullOrWhiteSpace(prefix))
                    bat.AppendLine("timeout 2 > NUL");
            }

            this.batchBuffer = bat.ToString();
        }

        public void SaveToFile(string filepath)
        {
            if (string.IsNullOrEmpty(filepath))
            {
                throw new ArgumentException("Filepath cannot be empty", nameof(filepath));
            }

            File.WriteAllText(filepath, this.batchBuffer);
        }
    }
}
