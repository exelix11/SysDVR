#include "Scenes.hpp"
#include "Common.hpp"

#include "../translaton.hpp"

namespace scenes {
	// From fatal error resources
	extern Image::Img errorQrUrl;

	void NoConnection()
	{
		SetupMainWindow("No connection");
		CenterImage(SysDVRLogo, 1);

		CenterText(Strings::Error.SysmoduleConnectionFailed);
		CenterText(Strings::Error.SysmoduleConnectionTroubleshoot);

		ImGui::NewLine();
		CenterText(Strings::Error.SysmoduleConnectionTroubleshootLink);
		CenterImage(errorQrUrl, 0.8f);
		CenterText("github.com/exelix11/SysDVR/wiki/Troubleshooting");

		auto selection = ImGuiCenterButtons<std::string_view>({ Strings::Error.FailExitButton, Strings::Main.OptPatchManager });
		
		if (selection == 0)
			app::RequestExit();
		else if (selection == 1)
			app::SetNextScene(Scene::DvrPatches);

		ImGui::End();
	}
}