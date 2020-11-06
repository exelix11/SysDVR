using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace SysDVR.Client.Player
{
	static class Helper
	{
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool SetDllDirectory(string lpPathName);

		/*
			Ffmpeg bindings use native load library method and not DllImport this means 
			loading any ffmpeg lib on windows will fail unless we manually add the native
			folder to the search path. Only x64 as ffmpeg builds are not provided for x86.

			TODO: eventually fork the ffmpeg bindings and change it DllImport
		 */
		public static void WindowsDllWorkadround() 
		{
			if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				return;

			if (RuntimeInformation.ProcessArchitecture != Architecture.X64)
				return;

			if (!SetDllDirectory(@"runtimes\win-x64\native"))
				Console.WriteLine("Warning: Couldn't set Dll search directory, loading ffmpeg may fail.");
		}
	}
}
