#include "Scenes.hpp"
#include "Common.hpp"
#include "../ipc.h"

namespace scenes {
	void DevTestScene()
	{
		SetupMainWindow("DevTest");
		CenterImage(SysDVRLogo, 1);

		CenterText("These options are for debug and should not be used normally.");
		
		ImGui::NewLine();
		if (ImGui::Button("Crash"))
			SysDVRDebugCrash();

		ImGui::NewLine();
		ImGui::NewLine();

		if (ImGui::Button("    No audio batch    "))
			SysDvrSetAudioBatchingOverride(1);

		ImGui::SameLine();

		if (ImGui::Button("    Audio batch 1    "))
			SysDvrSetAudioBatchingOverride(2);

		ImGui::SameLine();
		if (ImGui::Button("    Audio batch 2    "))
			SysDvrSetAudioBatchingOverride(3);

		ImGui::NewLine();

		if (ImGui::Button("Back"))
			app::ReturnToPreviousScene();

		ImGui::End();
	}
}