#pragma once
#include <string>	
#include <vector>

namespace Strings
{
	struct MainPageTable
	{
		std::string ModeUsbTitle = "USB";
		std::string ModeUsb = "Use this mode to stream to the SysDVR-Client application via USB.\n"
			"To setup SysDVR-Client on your pc refer to the guide on Github";

		std::string ModeTcpTitle = "TCP Bridge";
		std::string ModeTcp = "Stream to the SysDVR-Client application via network. This is more stable than the simple network mode.\n"
			"To setup SysDVR-Client on your pc refer to the guide on Github\n"
			"For network modes it's recommended to use a LAN adapter.";

		std::string ModeRtspTitle = "Simple network mode (RTSP)";
		std::string ModeRtsp = "Stream directly to any video player application via RTSP.\n"
			"Once you enable and apply it open rtsp://{}:6666/ with any video player like mpv or vlc.\n"
			"This mode doesn't require SysDVR-Client on your PC.\n"
			"For network modes it's recommended to use a LAN adapter.";

		std::string ModeDisabled = "Stop streaming";

		std::string ConsoleIPPlcaceholder = "<console IP address>";

		std::string SelectMode = "Select mode";

		std::string OptGuide = "Guide";
		std::string OptSetDefault = "Set current mode as default on boot";
		std::string OptPatchManager = "dvr-patches manager";
		std::string OptSave = "Save and exit";

		std::string ActiveMode = "Enabled";
		std::string DefaultMode = "On boot";

		std::string OptTerminateSysmodule = "Terminate SysDVR";
		std::string WarnSysmoduleKill = "This will terminate the SysDVR process and free its memory for other sysmodules to use.\nNote that this option may cause conflicts with third-party sysmodule managers.";
		std::string ContinueQuestion = "Do you want to continue?";
		std::string Yes = "Yes";
		std::string No = "No";

		std::string OptTryStart = "Try to start SysDVR";
		std::string StartFailed = "Launching SysDVR failed. Error code: ";
		std::string StartSuccess = "SysDVR was started. Close and open this app again.";
	};

	struct GuideTable
	{
		std::string GuideTitle = "The guide is hosted on Github:";
	};

	struct ErrorTable
	{
		std::string FailedToDetectMode = "Failed to detect the current SysDVR mode";
		std::string InvalidMode = "The reported SysDVR mode is not valid";
		std::string TroubleshootReboot = "Try rebooting your console";
		std::string ModeChangeFailed = "Failed to change mode";
		std::string BootModeChangeFailed = "Couldn't set boot mode";
		std::string TroubleshootBootMode = "Try checking your SD card for corruption";

		std::string SysmoduleConnectionFailed = "Couldn't connect to SysDVR.";
		std::string SysmoduleConnectionTroubleshoot = "If you just installed it reboot, otherwise wait a bit and try again.\nThis is normal if you're using an USB-Only version of SysDVR.";
		std::string SysmoduleConnectionTroubleshootLink = "For support check the troubleshooting page:";
		
		std::string FailExitButton = "Click or press + to exit";

		std::string SysdvrVersionError = "Couldn't get the SysDVR version";

		std::string OlderVersion = "You're using an outdated version of SysDVR";
		std::string NewerVersion = "You're using a newer version of SysDVR";

		std::string VersionTroubleshoot = "Please download the latest settings app from github.";

		std::string NotInstalled = "SysDVR is not installed on your console";
		std::string NotInsalledSecondLine = "The file /atmosphere/contents/00FF0000A53BB665/exefs.nsp was not found on your SD card, this means you did not extract the SysDVR zip correctly. Fix the issue then reboot the console.";

		std::string SysmoduleConnectionTroubleshootFull = "Wait a bit and try again if you still can't get past this screen try rebooting your console.";
		std::string SysmoduleConnectionTroubleshootUsbOnly = "You're using a USB-Only version of SysDVR. The SysDVR-Settings app does not support it.";
		std::string DiagProcessStatusOn = "The sysmodule process seems to be running.";
		std::string DiagProcessStatusOff = "The sysmodule process is not running.";
	};

	struct PatchesTable
	{
		std::string CurlError = "Curl initialization failed";
		std::string CurlGETFailed = "Curl GET failed";
		std::string CurlNoData = "No data received";
		std::string ParseReleaseFailure = "Failed to parse release info";
		std::string ParseTagFailure = "Failed to parse github tags info";
		std::string ParseTagCommitFailure = "Failed to find commit for tag";
		std::string ParseDownloadFailure = "Failed to find the download link for the release";
		
		std::string NoLinkFound = "No update source found";
		std::string ZipExtractFail = "Failed to open zip archive";
		
		std::string LatestVer = "You're already using latest version of dvr-patches.";
		std::string NewVerAvail = "New version of dvr-patches available:";
		std::string DownloadOk = "Update downloaded.";
		
		std::string Title = "dvr-patches manager";
		std::string Loading = "Loading...";
		
		std::string Description = "dvr-patches are system patches that allow to stream most incompatible games with SysDVR, a few games are reported to crash, you can read mone on the issue tracker on https://github.com/exelix11/dvr-patches.\n"
			"You can also manually download the zip file from the GitHub repo with a computer.";

		std::string Status = "dvr-patches status:";
		
		std::string StatusNotInstalled = "not installed";
		std::string StatusUnknownVersion = "installed, version unknown";
		std::string StatusInstalled = "installed commit %s";

		std::string SdcardPath = "Sdcard path:";
		std::string UninstallButton = "Uninstall";
		std::string DownloadButton = "Download and install";

		std::string RebootWarning = "To apply the changes you need to reboot your console    ";

		std::string RebootButton = "Reboot now";
		
		std::string BackButton = "Go back";
		std::string SearchLatestButton = "Search for latest patches on GitHub";
	};

	struct ConnectingTable
	{
		std::string Title = "Connecting to SysDVR...";

		std::string Description = "If you just turned on your console this may take up to 20 seconds.";

		std::string Troubleshoot1 = "If you can't get past this screen SysDVR not running";
		std::string Troubleshoot2 = "Make sure your setup is correct and reboot your console.";
	};

	enum class GlyphRange
	{
		NotSpecified,
		ChineseSimplifiedCommon,
		Cyrillic,
		Default,
		Greek,
		Japanese,
		Korean,
		Thai,
		Vietnamese,
		Arabic
	};

	extern std::string FontName;
	extern GlyphRange ImguiFontGlyphRange;

	extern MainPageTable Main;
	extern GuideTable Guide;
	extern ErrorTable Error;
	extern PatchesTable Patches;
	extern ConnectingTable Connecting;

	void LoadTranslationForSystemLanguage();
	void IterateAllStringsForFontBuilding(void*, void (*)(void*, std::string_view));

	void ResetStringTable();
	
	// For development only
	void SerializeCurrentLanguage();
}