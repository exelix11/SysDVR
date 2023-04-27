#include "Scenes.hpp"
#include "Common.hpp"
#include "../ipc.h"

namespace {
	Result LastError;
	UserOverrides Overrides;

	bool EnableOverrides;
	int MaxSeqDrops;

	const char* SeqDropsComboOptions[] = {
		"0 (Feature disabled)",
		"1", "2", "3", "4 (default)",
		"5", "6", "7", "8", "9", 
		"10 (expect visual artifacts)"
	};

	constexpr int SeqDropsComboCount = sizeof(SeqDropsComboOptions) / sizeof(SeqDropsComboOptions[0]);
}

namespace scenes {
	void InitDevScene()
	{
		LastError = SysDvrGetUserOverrides(&Overrides);
		if (R_SUCCEEDED(LastError))
		{
			EnableOverrides = Overrides.Enabled;
			MaxSeqDrops = Overrides.StaticDropThreshold;
		}
	}

	void SaveChanges() 
	{
		Overrides.Enabled = EnableOverrides;
		Overrides.StaticDropThreshold = MaxSeqDrops;
		LastError = SysDvrSetUserOverrides(&Overrides);

		if (R_SUCCEEDED(LastError)) {
			// Check if it was successful
			InitDevScene();
		}
	}

	void DevTestScene()
	{
		SetupMainWindow("Advanced options", UI::DefaultFramePadding);
		
		ImGui::SetCursorPosY(30);
		CenterImage(SysDVRLogo, .6f);

		ImGui::TextWrapped("These are advanced options to tweak communication and capture behavior, if you're not sure don't use them. These options are not saved and they're cleared when you reboot your console.");

		ImGui::NewLine();
		
		if (R_FAILED(LastError))
		{
			ImGui::Text("Last operation failed: 0x%x", LastError);
			ImGui::NewLine();
		}

		ImGui::Checkbox("Enable custom options, if disabled the following won't have any effect", &EnableOverrides);

		ImGui::NewLine();

		ImGui::TextWrapped("Audio batching: When enabled send audio data less often by grouping together small packets, each packet is about a frame worth of audio so delay is minimal.");
		if (ImGui::RadioButton("Disabled", Overrides.AudioBatching == 0))
			Overrides.AudioBatching = 0;

		ImGui::SameLine();
		if (ImGui::RadioButton("1 Packet (Default)", Overrides.AudioBatching == 1))
			Overrides.AudioBatching = 1;

		ImGui::SameLine();
		if (ImGui::RadioButton("2 Packets", Overrides.AudioBatching == 2))
			Overrides.AudioBatching = 2;

		ImGui::NewLine();

		ImGui::TextWrapped("Max sequential static packet drops: When enabled sysdvr will not send big repeated keyframes, this fixes delay issues caused by games showing static images. Big values may cause compression artifacts in the stream.");
		ImGui::Combo("", &MaxSeqDrops, SeqDropsComboOptions, SeqDropsComboCount);

		ImGui::NewLine();

		switch (ImGuiCenterButtons({ "   Refresh   ", "   Save   ", "   Back   " }))
		{
		case 0:
			InitDevScene();
			break;
		case 1:
			SaveChanges();
			break;
		case 2:
			app::ReturnToPreviousScene();
			break;
		}

		ImGui::End();
	}
}