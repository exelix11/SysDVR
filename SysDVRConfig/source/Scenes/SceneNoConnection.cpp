#include "Scenes.hpp"
#include "Common.hpp"

namespace scenes {
	// From fatal error resources
	extern Image::Img errorQrUrl;

	void NoConnection()
	{
		SetupMainWindow("No connection");
		CenterImage(SysDVRLogo, 1);

		CenterText("Couldn't connect to SysDVR.");
		CenterText("If you just installed it reboot, otherwise wait a bit and try again.\nThis is normal if you're using an USB-Only version of SysDVR.");

		ImGui::NewLine();
		CenterText("For support check the troubleshooting page:");
		CenterImage(errorQrUrl, 0.8f);
		CenterText("github.com/exelix11/SysDVR/wiki/Troubleshooting");

		auto selection = ImGuiCenterButtons({ "  Click or press + to exit  ", "   dvr-patches manager   " });
		
		if (selection == 0)
			app::RequestExit();
		else if (selection == 1)
			app::SetNextScene(Scene::DvrPatches);

		ImGui::End();
	}
}