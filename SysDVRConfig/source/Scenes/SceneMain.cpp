#include "Scenes.hpp"
#include "Common.hpp"

#include "../translaton.hpp"
#include "../ipc.h"
#include "../Platform/fs.hpp"

#ifdef WIN32
	#include <WinSock2.h>
	#pragma comment(lib, "ws2_32.lib")
	#undef max
	#undef min
#endif

namespace {
	Image::Img ModeRtsp;
	Image::Img ModeTcp;
	Image::Img ModeUsb;

	u32 InitialMode;
	u32 BootMode;
	u32 CurrentMode;
	
	std::string RtspDescription;
	std::string TcpDescription;
	std::string UsbDescription;

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

	constexpr float ModeButtonW = 1280 * 0.8f;
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
				const ImVec2 sz = ImGui::CalcTextSize(Strings::Main.ActiveMode.c_str()) + ImVec2{ 6, 0 };
				pos.x -= sz.x;
				window->DrawList->AddRectFilled(pos, pos + sz, 0xFF008800);
				ImGui::RenderText(pos + ImVec2{ 3,0 }, Strings::Main.ActiveMode.c_str());
			}
			if (OnBoot)
			{
				const ImVec2 sz = ImGui::CalcTextSize(Strings::Main.DefaultMode.c_str()) + ImVec2{ 6, 0 };
				pos.x -= sz.x;
				window->DrawList->AddRectFilled(pos, pos + sz, 0xFF000088);
				ImGui::RenderText(pos + ImVec2{ 3,0 }, Strings::Main.DefaultMode.c_str());
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
	
	RtspDescription = Strings::Main.ModeRtsp;
	TcpDescription = Strings::Main.ModeTcp;
	UsbDescription = Strings::Main.ModeUsb;

	if (RtspDescription.find("{}") != std::string::npos) {
		char hostname[128] = { };
		if (gethostname(hostname, sizeof(hostname)) || !strcmp(hostname, "1.0.0.127"))
			RtspDescription.replace(RtspDescription.find("{}"), 2, hostname);
		else
		{
			RtspDescription.replace(RtspDescription.find("{}"), 2, Strings::Main.ConsoleIPPlcaceholder);
		}
	}

	Result rc = SysDvrGetMode(&CurrentMode);
	if (R_FAILED(rc)) {
		app::FatalErrorWithErrorCode(Strings::Error.FailedToDetectMode, rc);
		return;
	}

	if (CurrentMode == TYPE_MODE_INVALID)
		app::FatalError(Strings::Error.InvalidMode, Strings::Error.TroubleshootReboot);

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

			app::FatalErrorWithErrorCode(Strings::Error.ModeChangeFailed, rc);
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
	if (ModeButton(Strings::Main.ModeRtspTitle, RtspDescription, ModeRtsp, CurrentMode == TYPE_MODE_RTSP, BootMode == TYPE_MODE_RTSP))
		if (!SetMode(TYPE_MODE_RTSP))
			return;

	ImGui::SetCursorPosX(1280 / 2 - ModeButtonW / 2);
	if (ModeButton(Strings::Main.ModeTcpTitle, TcpDescription, ModeTcp, CurrentMode == TYPE_MODE_TCP, BootMode == TYPE_MODE_TCP))
		if (!SetMode(TYPE_MODE_TCP))
			return;

	ImGui::SetCursorPosX(1280 / 2 - ModeButtonW / 2);
	if (ModeButton(Strings::Main.ModeUsbTitle, UsbDescription, ModeUsb, CurrentMode == TYPE_MODE_USB, BootMode == TYPE_MODE_USB))
		if (!SetMode(TYPE_MODE_USB))
			return;

	ImGui::SetCursorPosX(1280 / 2 - ModeButtonW / 2);
	if (ImGui::Button(Strings::Main.ModeDisabled.c_str(), {ModeButtonW , 0}))
		if (!SetMode(TYPE_MODE_NULL))
			return;

	ImGui::SetCursorPosY(ImGui::GetCursorPosY() + 20);
	switch (ImGuiCenterButtons<std::string_view>({ Strings::Main.OptGuide, Strings::Main.OptSetDefault, Strings::Main.OptPatchManager, Strings::Main.OptSave}))
	{
	case 0:
		app::SetNextScene(Scene::Guide);
		break;
	case 1:
		if (!SetDefaultBootMode(CurrentMode))
			app::FatalError(Strings::Error.BootModeChangeFailed, Strings::Error.TroubleshootBootMode);
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