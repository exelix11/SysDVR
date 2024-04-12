#include <stdio.h>
#include <stdlib.h>
#include <string>

#include "UI/UI.hpp"
#include "Platform/Platform.hpp"
#include "Platform/fs.hpp"
#include "UI/imgui/imgui.h"
#include "UI/Image.hpp"
#include "ipc.h"

#include "Scenes/Scenes.hpp"
#include "Scenes/Common.hpp"

#include "translaton.hpp"

Image::Img SysDVRLogo;

namespace {
	Scene currentScene = Scene::FatalError;
	Scene returnTo = Scene::FatalError;

	std::string_view statusMessage = "Fatal error";
	std::string_view errorSecondline = "This should never happen, try rebooting your console";

	std::string formattedError;

	bool waitOnExit = false;

	void ImguiBindController()
	{
		ImGuiIO& io = ImGui::GetIO();

		io.NavInputs[ImGuiNavInput_DpadDown] = g_gamepad.buttons[GLFW_GAMEPAD_BUTTON_DPAD_DOWN];
		io.NavInputs[ImGuiNavInput_DpadUp] = g_gamepad.buttons[GLFW_GAMEPAD_BUTTON_DPAD_UP];
		io.NavInputs[ImGuiNavInput_DpadLeft] = g_gamepad.buttons[GLFW_GAMEPAD_BUTTON_DPAD_LEFT];
		io.NavInputs[ImGuiNavInput_DpadRight] = g_gamepad.buttons[GLFW_GAMEPAD_BUTTON_DPAD_RIGHT];

		io.NavInputs[ImGuiNavInput_Activate] = g_gamepad.buttons[GLFW_GAMEPAD_BUTTON_A];
		io.NavInputs[ImGuiNavInput_Cancel] = g_gamepad.buttons[GLFW_GAMEPAD_BUTTON_B];
	}
		
	Result ConnectToSysmodule()
	{
		UI::StartFrame();

		SetupMainWindow("Connecting...");
		CenterImage(SysDVRLogo, 1);

		ImGui::NewLine();
		ImGui::NewLine();
		ImGui::NewLine();

		CenterText(Strings::Connecting.Title);
		CenterText(Strings::Connecting.Description);
		ImGui::NewLine();
		CenterText(Strings::Connecting.Troubleshoot1);
		CenterText(Strings::Connecting.Troubleshoot2);

		ImGui::End();
		UI::EndFrame();

		return SysDvrConnect();
	}
}

namespace scenes {
	Image::Img errorQrUrl;
	
	void InitFatalError() 
	{
		errorQrUrl = Image::Img(ASSET("troubleshooting.png"));
	}
	
	void FatalError()
	{
		SetupMainWindow("Fatal error");
		CenterImage(SysDVRLogo, 1);

		CenterText(statusMessage);
		if (errorSecondline.size() > 0) {
			ImGui::PushTextWrapPos();
			ImGui::TextUnformatted(errorSecondline.data(), errorSecondline.data() + errorSecondline.size());
			ImGui::PopTextWrapPos();
		}

		ImGui::NewLine();
		CenterText(Strings::Error.SysmoduleConnectionTroubleshootLink);
		CenterImage(errorQrUrl, 0.8f);
		CenterText("github.com/exelix11/SysDVR/wiki/Troubleshooting");

		ImGui::NewLine();
		if (ImGuiCenterButtons<std::string_view>({ Strings::Error.FailExitButton }) != -1)
			app::RequestExit();
		
		ImGui::End();
	}
}

namespace app {
	void SetNextScene(Scene s)
	{
		returnTo = currentScene;
		currentScene = s;
	}

	void ReturnToPreviousScene() 
	{
		currentScene = returnTo;
		returnTo = Scene::FatalError;
	}
	
	void FatalErrorWithErrorCode(std::string_view message, uint32_t rc)
	{
		if (rc == 0)
			formattedError = "No error code specified";
		else
			formattedError = "Error code: " + std::to_string(2000 + R_MODULE(rc)) + "-" + std::to_string(R_DESCRIPTION(rc));

		app::FatalError(message, formattedError);
	}

	void FatalError(std::string_view message, std::string_view secondline)
	{
		SetNextScene(Scene::FatalError);
		statusMessage = message;
		errorSecondline = secondline;
	}

	void SetWaitOnExit(bool shouldWait)
	{
		waitOnExit = shouldWait;
	}

	void RequestExit()
	{
		Glfw::SetShouldClose();
	}	
}

int main(int argc, char* argv[])
{
	Platform::Init();

	Strings::LoadTranslationForSystemLanguage();

	if (!UI::Init())
	{
		Platform::Exit();
		return -1;
	}
	
	Platform::AfterInit();

	SysDVRLogo = Image::Img(ASSET("logo.png"));
	
	currentScene = Scene::ModeSelect;

	if (!fs::Exists(SDMC "/atmosphere/contents/00FF0000A53BB665/exefs.nsp"))
	{
		app::FatalError(Strings::Error.NotInstalled, Strings::Error.NotInsalledSecondLine);
		goto mainloop;
	}

	{
		Result rc = ConnectToSysmodule();
		if (R_FAILED(rc)) {
			app::SetNextScene(Scene::NoConnection);
			goto dvrNotConnected;
		}

		u32 version;
		rc = SysDvrGetVersion(&version);
		if (R_FAILED(rc)) {
			app::FatalErrorWithErrorCode(Strings::Error.SysdvrVersionError, rc);
			goto mainloop;
		}

		if (version > SYSDVR_IPC_VERSION) {
			app::FatalError(Strings::Error.NewerVersion, Strings::Error.VersionTroubleshoot);
			goto mainloop;
		}
		else if (version < SYSDVR_IPC_VERSION) {
			app::FatalError(Strings::Error.OlderVersion, Strings::Error.VersionTroubleshoot);
			goto mainloop;
		}
	}

	scenes::InitModeSelect();
	scenes::InitGuide();

dvrNotConnected:

	scenes::InitDvrPatches();

mainloop:
	// Always init the fatal error scene
	scenes::InitFatalError();
	
	while (Glfw::MainLoop())
	{
		Platform::GetInputs();
		ImguiBindController();
		Platform::ImguiBindings();

		UI::StartFrame();

		// Render background
		ImGui::SetNextWindowSize({ UI::WindowWidth , UI::WindowHeight });
		ImGui::SetNextWindowPos({ 0, 0 });

		ImGui::Begin("_background_", 0, ImGuiWindowFlags_NoInputs | ImGuiWindowFlags_NoDecoration);
		
		UI::SmallFont();

		switch (currentScene)
		{
		case Scene::ModeSelect:
			scenes::ModeSelect();
			break;
		case Scene::Guide:
			scenes::Guide();
			break;
		case Scene::FatalError:
			scenes::FatalError();
			break;
		case Scene::DvrPatches:
			scenes::DvrPatches();
			break;
		case Scene::NoConnection:
			scenes::NoConnection();
			break;
		default:
			Glfw::SetShouldClose();
			break;
		}

		UI::PopFont();
		ImGui::End();

		UI::EndFrame();
		Platform::Sleep(1 / 30.0f * 1000);
	}

	scenes::DeinitDvrPatches();

	SysDvrClose();

	if (waitOnExit)
		Platform::Sleep(2000);

	UI::Exit();
	Platform::Exit();
	return EXIT_SUCCESS;
}
