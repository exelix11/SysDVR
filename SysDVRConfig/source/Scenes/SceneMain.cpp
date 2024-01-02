#include "Scenes.hpp"
#include "Common.hpp"

#include "../ipc.h"
#include "../Platform/fs.hpp"

namespace {
	Image::Img ModeRtsp;
	Image::Img ModeTcp;
	Image::Img ModeUsb;

	u32 InitialMode;
	u32 BootMode;
	u32 CurrentMode;
	
	std::string RtspDescription = "Stream directly to any video player application via RTSP.\n"
		"Once you enable and apply it open rtsp://{}:6666/ with any video player like mpv or vlc.\n"
		"This mode doesn't require SysDVR-Client on your PC.\n"
		"For network modes it's recommended to use a LAN adapter.";

	const std::string TcpDescription = "Stream to the SysDVR-Client application via network. This is more stable than the simple network mode.\n"
		"To setup SysDVR-Client on your pc refer to the guide on Github\n"
		"For network modes it's recommended to use a LAN adapter.";

	const std::string UsbDescription = "Use this mode to stream to the SysDVR-Client application via USB.\n"
		"To setup SysDVR-Client on your pc refer to the guide on Github";

	u32 GetBootMode()
	{
		if (fs::Exists(SDMC "/config/sysdvr/usb"))
			return TYPE_MODE_USB;
		else if (fs::Exists(SDMC "/config/sysdvr/rtsp"))
			return TYPE_MODE_RTSP;
		else if (fs::Exists(SDMC "/config/sysdvr/tcp"))
			return TYPE_MODE_TCP;
		else
			return TYPE_MODE_NULL;
	}

	bool SetDefaultBootMode(u32 mode)
	{
		fs::Delete(SDMC "/config/sysdvr/usb");
		fs::Delete(SDMC "/config/sysdvr/tcp");
		fs::Delete(SDMC "/config/sysdvr/rtsp");

		fs::CreateDir(SDMC "/config/");
		fs::CreateDir(SDMC "/config/sysdvr/");

		try {
			if (mode == TYPE_MODE_USB)
				fs::WriteFile(SDMC "/config/sysdvr/usb", { 'a' });
			else if (mode == TYPE_MODE_TCP)
				fs::WriteFile(SDMC "/config/sysdvr/tcp", { 'a' });
			else if (mode == TYPE_MODE_RTSP)
				fs::WriteFile(SDMC "/config/sysdvr/rtsp", { 'a' });
		}
		catch (std::exception&)
		{
			return false;
		}
		return true;
	}

	constexpr float ModeButtonW = 1280 * 0.7f;
	bool ModeButton(std::string_view title, std::string_view description, Image::Img& image, bool Active, bool OnBoot)
	{
		ImGuiWindow* window = ImGui::GetCurrentWindow();
		if (window->SkipItems)
			return false;

		ImGuiContext& g = *GImGui;
		const ImGuiStyle& style = g.Style;
		const ImGuiID id = window->GetID(title.data(), title.data() + title.size());
		const ImVec2 label_size = ImGui::CalcTextSize(title.data(), title.data() + title.size(), false);
		const ImVec2 desc_size = ImGui::CalcTextSize(description.data(), description.data() + description.size(), false, ModeButtonW - image.Size().x - style.FramePadding.x * 2);

		ImVec2 pos = window->DC.CursorPos;

		ImVec2 size = ImGui::CalcItemSize({ ModeButtonW, 0 },
			image.Size().x + label_size.x + desc_size.x + style.FramePadding.x * 2.0f,
			std::max(image.Size().y, desc_size.y) + label_size.y + style.FramePadding.y * 4.0f);

		const ImRect bb(pos, pos + size);
		ImGui::ItemSize(size, style.FramePadding.y);
		if (!ImGui::ItemAdd(bb, id))
			return false;

		bool hovered, held;
		bool pressed = ImGui::ButtonBehavior(bb, id, &hovered, &held, 0);
		if (pressed)
			ImGui::MarkItemEdited(id);

		const auto contentStart = bb.Min + style.FramePadding;

		// Render
		const ImU32 col = ImGui::GetColorU32((held && hovered) ? ImGuiCol_ButtonActive : hovered ? ImGuiCol_ButtonHovered : ImGuiCol_Button);
		ImGui::RenderNavHighlight(bb, id);
		ImGui::RenderFrame(bb.Min, bb.Max, col, true, style.FrameRounding);
		ImGui::RenderText(contentStart, title.data(), title.data() + title.size(), false);
		ImGui::RenderTextWrapped(contentStart + style.FramePadding + ImVec2{ image.Size().x, label_size.y }, description.data(), description.data() + description.size(), ModeButtonW - image.Size().x - style.FramePadding.x * 2);

		const ImVec2 imagePos = { contentStart.x , contentStart.y + style.FramePadding.y + label_size.y };
		window->DrawList->AddImage(image, imagePos, imagePos + image.Size());

		if (Active || OnBoot)
		{
			ImVec2 pos = bb.GetTR();
			if (Active)
			{
				const ImVec2 sz = ImGui::CalcTextSize("Enabled") + ImVec2{ 6, 0 };
				pos.x -= sz.x;
				window->DrawList->AddRectFilled(pos, pos + sz, 0xFF008800);
				ImGui::RenderText(pos + ImVec2{ 3,0 }, "Enabled");
			}
			if (OnBoot)
			{
				const ImVec2 sz = ImGui::CalcTextSize("On boot") + ImVec2{ 6, 0 };
				pos.x -= sz.x;
				window->DrawList->AddRectFilled(pos, pos + sz, 0xFF000088);
				ImGui::RenderText(pos + ImVec2{ 3,0 }, "On boot");
			}
		}

		IMGUI_TEST_ENGINE_ITEM_INFO(id, label, window->DC.LastItemStatusFlags);
		return pressed;
	}
}

void scenes::InitModeSelect() 
{
	ModeRtsp = Image::Img(ASSET("ModeRtsp.png"));
	ModeTcp = Image::Img(ASSET("ModeTcp.png"));
	ModeUsb = Image::Img(ASSET("ModeUsb.png"));

	char hostname[128] = "<console IP address>";
#ifdef __SWITCH__	
	if (gethostname(hostname, sizeof(hostname)) || !strcmp(hostname, "1.0.0.127"))
		strcpy(hostname, "<console IP address>");
#endif
	RtspDescription.replace(RtspDescription.find("{}"), 2, hostname);
	
	Result rc = SysDvrGetMode(&CurrentMode);
	if (R_FAILED(rc)) {
		app::FatalErrorWithErrorCode("Couldn't get the SysDVR mode", rc);
		return;
	}

	if (CurrentMode == TYPE_MODE_INVALID)
		app::FatalError("Couldn't get the current SysDVR mode", "Try rebooting your console");

	BootMode = GetBootMode();
	InitialMode = CurrentMode;
}

void scenes::ModeSelect()
{

	constexpr auto SetMode = [](u32 mode) -> bool
	{
		Result rc = SysDvrSetMode(mode);

		if (R_FAILED(rc))
		{
			// This is called while drawing, finish rendering the frame
			ImGui::End();
			UI::EndFrame();

			app::FatalErrorWithErrorCode("Couldn't change mode", rc);
			return false;
		}

		CurrentMode = mode;
		return true;
	};

	SetupMainWindow("Select mode");
	auto logoPosition = ImGui::GetCursorPosY() + 12;
	ImGui::SetCursorPosY(logoPosition);
	CenterImage(SysDVRLogo, .6f);

	// Debug button is invisible and overlaps the logo
	//ImGui::SetCursorPosY(logoPosition);
	//if (DebugButton())
	//	app::SetNextScene(Scene::DevScene);

	ImGui::SetCursorPosX(1280 / 2 - ModeButtonW / 2);
	if (ModeButton("Simple network mode", RtspDescription, ModeRtsp, CurrentMode == TYPE_MODE_RTSP, BootMode == TYPE_MODE_RTSP))
		if (!SetMode(TYPE_MODE_RTSP))
			return;

	ImGui::SetCursorPosX(1280 / 2 - ModeButtonW / 2);
	if (ModeButton("TCP Bridge", TcpDescription, ModeTcp, CurrentMode == TYPE_MODE_TCP, BootMode == TYPE_MODE_TCP))
		if (!SetMode(TYPE_MODE_TCP))
			return;

	ImGui::SetCursorPosX(1280 / 2 - ModeButtonW / 2);
	if (ModeButton("USB", UsbDescription, ModeUsb, CurrentMode == TYPE_MODE_USB, BootMode == TYPE_MODE_USB))
		if (!SetMode(TYPE_MODE_USB))
			return;

	ImGui::SetCursorPosX(1280 / 2 - ModeButtonW / 2);
	if (ImGui::Button("Stop streaming", { ModeButtonW , 0 }))
		if (!SetMode(TYPE_MODE_NULL))
			return;

	ImGui::SetCursorPosY(ImGui::GetCursorPosY() + 20);
	switch (ImGuiCenterButtons({ "Guide", "Set current mode as default on boot", "dvr-patches manager", "Save and exit"}))
	{
	case 0:
		app::SetNextScene(Scene::Guide);
		break;
	case 1:
		if (!SetDefaultBootMode(CurrentMode))
			app::FatalError("Couldn't set boot mode", "Try checking your SD card for corruption");
		else
			BootMode = CurrentMode;
		break;
	case 2:
		app::SetNextScene(Scene::DvrPatches);
		break;
	case 3:
		// Mode was changed, after deinitializing the sysdvr port wait a little
		// This fixes a possible race condition when the user opens the settings app
		// right after closing it and sysdvr will miss the request causing the app to hang
		if (InitialMode != CurrentMode)
			app::SetWaitOnExit(true);
		
		app::RequestExit();
		break;
	default:
		break;
	}
	
	ImGui::End();
}