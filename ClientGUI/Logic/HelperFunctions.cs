using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SysDVRClientGUI.Logic
{
    internal static class HelperFunctions
    {
        public static string GetVersionString()
        {
            var Version = typeof(HelperFunctions).Assembly.GetName().Version;
            if (Version == null) return "<unknown version>";
            StringBuilder str = new();
            str.Append(Version.Major);
            str.Append('.');
            str.Append(Version.Minor);

            if (Version.Revision != 0)
            {
                str.Append('.');
                str.Append(Version.Revision);
            }

            return str.ToString();
        }

        #region Find Dotnet Version
        // We want to detect the dotnet version, there are simple registry keys but they don't seem documented
        // ms just says to invoke dotnet https://learn.microsoft.com/en-us/dotnet/core/install/how-to-detect-installed-versions?pivots=os-windows

        const string DotnetPath = @"SOFTWARE\dotnet\Setup\InstalledVersions\";

        static readonly Regex VersionRegex = new(@"^(\d+)\.(\d+)(\.(\d+))?", RegexOptions.Compiled);

        public static int FindDotnet(out string path, out bool is32Bit)
        {
            // Search the 32bit hive for a x64 version
            path = ParseDotnetRegistry(DotnetPath + "x64", out var version);
            if (path != null)
            {
                is32Bit = false;
                return version;
            }

            // Search a 32bit version to report the error
            path = ParseDotnetRegistry(DotnetPath + "x86", out version);
            if (path != null)
            {
                is32Bit = true;
                return version;
            }

            is32Bit = false;
            return 0;
        }

        // This searches the 32-bit hive because it seems the 64-bit one has a different format, wtf ?
        private static string ParseDotnetRegistry(string Regpath, out int version)
        {
            version = 0;
            try
            {
                using (var HKLM = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    var key = HKLM.OpenSubKey(Regpath, false);

                    if (key == null)
                        return null;

                    var path = key.GetValue("InstallLocation") as string;
                    path = Path.Combine(path, "dotnet.exe");

                    if (!File.Exists(path))
                        return null;

                    var hosts = key.OpenSubKey("sharedfx")?.OpenSubKey("Microsoft.NETCore.App");

                    if (hosts == null)
                        return null;

                    var major = hosts.GetValueNames()
                        .Select(x => VersionRegex.Match(x))
                        .Where(x => x.Success)
                        .Select(x => int.Parse(x.Groups[1].Value))
                        .OrderByDescending(x => x)
                        .FirstOrDefault();

                    if (major == default)
                        return null;

                    version = major;
                    return path;
                }
            }
            catch { return null; }
        }
        #endregion

        public static void CopyFilesRecursively(string sourcePath, string targetPath)
        {
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
            }
        }
    }
}
