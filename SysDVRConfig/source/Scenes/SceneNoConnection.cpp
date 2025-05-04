#include "Scenes.hpp"
#include "Common.hpp"

#include "../translaton.hpp"
#include "../ipc.h"

namespace 
{
	SYSDVR_VARIANT detectedVariant;
	bool isProcessRunning;

	bool canStartSysdvr;
	std::string startSysdvrResult;
}

namespace scenes {
	// From fatal error resources
	extern Image::Img errorQrUrl;

	void InitNoConnection()
	{
		// cache the process running value for this scene
		detectedVariant = SysDvrDetectVariantCached(SDMC SYSDVR_EXEFS_PATH);
		isProcessRunning = SysDvrProcIsRunning();
		
		canStartSysdvr = 
			detectedVariant == SYSDVR_VARIANT_FULL && // Do not allow starting usb-only variants since we can't manage them
			SysDvrProcManagerIsInitialized() && !SysDvrProcIsRunning();
	}

	static void TryStartSysDVR() 
	{
		// Limit to one single attempt
		canStartSysdvr = false;

		Result rc = SysDvrProcLaunch();
		if (R_FAILED(rc))
		{
			startSysdvrResult = Strings::Main.StartFailed + std::to_string(2000 + R_MODULE(rc)) + "-" + std::to_string(R_DESCRIPTION(rc));
		}

		startSysdvrResult = Strings::Main.StartSuccess;
	}

	void NoConnection()
	{
		SetupMainWindow("No connection");
		CenterImage(SysDVRLogo, 1);

		// Default screen of this page. Show diagnostics and if we know the current sysdvr is the full version allow the user to start it.
		if (startSysdvrResult == "")
		{
			CenterText(Strings::Error.SysmoduleConnectionFailed);

			if (detectedVariant == SYSDVR_VARIANT_USB_ONLY)
				CenterText(Strings::Error.SysmoduleConnectionTroubleshootUsbOnly);
			else if (detectedVariant == SYSDVR_VARIANT_FULL)
				CenterText(Strings::Error.SysmoduleConnectionTroubleshootFull);
			else
				CenterText(Strings::Error.SysmoduleConnectionTroubleshoot);

			ImGui::NewLine();

			if (isProcessRunning)
				CenterText(Strings::Error.DiagProcessStatusOn);
			else
				CenterText(Strings::Error.DiagProcessStatusOff);

			CenterText(Strings::Error.SysmoduleConnectionTroubleshootLink);
			CenterImage(errorQrUrl, 0.7f);
			CenterText("github.com/exelix11/SysDVR/wiki/Troubleshooting");

			int selection = canStartSysdvr ?
				ImGuiCenterButtons<std::string_view>({ Strings::Error.FailExitButton, Strings::Main.OptPatchManager, Strings::Main.OptTryStart }) :
				ImGuiCenterButtons<std::string_view>({ Strings::Error.FailExitButton, Strings::Main.OptPatchManager });

			if (selection == 0 || ImGui::GetIO().NavInputs[ImGuiNavInput_Menu])
				app::RequestExit();
			else if (selection == 1)
				app::SetNextScene(Scene::DvrPatches);
			else if (selection == 2)
				TryStartSysDVR();
		}
		else
		{
			// If we attempted to start sysdvr, regardless of the result, only show the exit button.

			ImGui::NewLine();

			CenterText(startSysdvrResult);

			ImGui::NewLine();

			int selection = ImGuiCenterButtons<std::string_view>({ Strings::Error.FailExitButton });

			if (selection == 0 || ImGui::GetIO().NavInputs[ImGuiNavInput_Menu])
				app::RequestExit();
		}

		ImGui::End();
	}
}