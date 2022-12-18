#include "Scenes.hpp"
#include "Common.hpp"
#include "../Platform/fs.hpp"

namespace {
	Image::Img guide;
	Image::Img guideFont;
}

namespace scenes {
	void InitGuide()
	{
		guide = Image::Img(ASSET("guide.png"));

		auto file = fs::OpenFile(ASSET("font.dat"));
		UI::DecodeFont(file.data(), file.size());
		guideFont = Image::Img(file);
	}
	
	void Guide()
	{
		static const char* str = "gghhefef";
		static int i = 0;
		static int j = 0;

		SetupMainWindow("Guide");
		CenterImage(SysDVRLogo, .6f);
		
		ImGui::NewLine();

		CenterText("The guide is hosted on Github:");
		CenterImage(guide);
		CenterText("github.com/exelix11/SysDVR/wiki/");

		ImGui::NewLine();
		
		if (ImGuiCenterButtons({ "    Back   " }) == 0)
		{
			app::SetNextScene(Scene::ModeSelect);
			i = 0;
			j = 0;
		}

		if (j && !AnyNavButtonPressed()) {
			if (i == 7)
				i = -1280;
			else ++i;
			j = 0;
		}
		else if (i >= 0 && ImGui::IsNavInputPressed(str[i] - 'a', ImGuiInputReadMode_Pressed)) {
			j = 1;
		}
		else if (i < 0) {
			ImGui::SetCursorPos({ 1000.0f + i, 400 });
			ImGui::Image(guideFont, guideFont.Size());
			i += 8;
			if (i > 0) i = 0;
		}
		else if (!j && AnyNavButtonPressed())
			i = 0;

		ImGui::End();
	}
}