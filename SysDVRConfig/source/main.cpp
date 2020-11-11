#include <stdio.h>
#include <stdlib.h>
#include <string>

#include "UI/UI.hpp"
#include "Platform/Platform.hpp"
#include "UI/imgui/imgui.h"
#include "UI/Image.hpp"
#include "Platform/fs.hpp"
#include "ipc.h"

static Image::Img SysDVRLogo;

static Image::Img ModeRtsp;
static Image::Img ModeTcp;
static Image::Img ModeUsb;

static Image::Img Guide;

static u32 GetBootMode()
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

static bool SetDefaultBootMode(u32 mode)
{
	fs::Delete(SDMC "/config/sysdvr/usb");
	fs::Delete(SDMC "/config/sysdvr/tcp");
	fs::Delete(SDMC "/config/sysdvr/rtsp");

	fs::CreateDirectory(SDMC "/config/");
	fs::CreateDirectory(SDMC "/config/sysdvr/");

	try {
		if (mode == TYPE_MODE_USB)
			fs::WriteFile(SDMC "/config/sysdvr/usb", {'a'});
		else if (mode == TYPE_MODE_TCP)
			fs::WriteFile(SDMC "/config/sysdvr/tcp", {'a'});
		else if (mode == TYPE_MODE_RTSP)
			fs::WriteFile(SDMC "/config/sysdvr/rtsp", {'a'});
	}
	catch (std::exception &ex)
	{
		return false;
	}
	return true;
}

bool AnyNavButtonPressed() 
{
	return  g_gamepad.buttons[GLFW_GAMEPAD_BUTTON_DPAD_DOWN] || g_gamepad.buttons[GLFW_GAMEPAD_BUTTON_DPAD_UP] ||
		    g_gamepad.buttons[GLFW_GAMEPAD_BUTTON_DPAD_LEFT] || g_gamepad.buttons[GLFW_GAMEPAD_BUTTON_DPAD_RIGHT];
}
 
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

 static inline void CenterImage(const Image::Img& img, float imageScale = 1)
 {
	 auto win = ImGui::GetWindowWidth();
	 ImGui::SetCursorPosX(win / 2 - img.Size().x * imageScale / 2);
	 ImGui::Image(img, img.Size() * imageScale);
 }

static inline void CenterText(const std::string_view text)
{
	auto win = ImGui::GetWindowWidth();
	auto sz = ImGui::CalcTextSize(text.data(), text.data() + text.size(), false, win);

	ImGui::SetCursorPosX(win / 2 - sz.x / 2);
	ImGui::TextUnformatted(text.data(), text.data() + text.size());
}

static void SetupMainWindow(const char* name, float logoscale = 1)
{
	ImGui::SetNextWindowSize({UI::WindowWidth, UI::WindowHeight});
	ImGui::SetNextWindowPos({0,0});

	ImGui::Begin(name, 0, ImGuiWindowFlags_NoDecoration | ImGuiWindowFlags_NoMove | ImGuiWindowFlags_NoResize);
	CenterImage(SysDVRLogo, logoscale);
}

static Result ConnectToSysmodule()
{
	UI::StartFrame();

	SetupMainWindow("Connecting...");	

	ImGui::NewLine();
	ImGui::NewLine();
	ImGui::NewLine();

	CenterText("Connecting to SysDVR.");
	CenterText("If you just turned on your console this may take up to 20 seconds");
	ImGui::NewLine();
	CenterText("If you can't get past this screen SysDVR is probably not running, make sure your setup is correct.");

	ImGui::End();
	UI::EndFrame();

	return SysDvrConnect();
}

static void FatalError(const std::string_view message, const std::string_view secondline)
{
	Image::Img url(ASSET("troubleshooting.png"));

	while (App::MainLoop())
	{
		Platform::GetInputs();

		// Exit by pressing Start (aka Plus)
		if (g_gamepad.buttons[GLFW_GAMEPAD_BUTTON_START] == GLFW_PRESS)
			App::SetShouldClose();

		UI::StartFrame();
		SetupMainWindow("Fatal error");

		CenterText(message);
		if (secondline.size() > 0)
			CenterText(secondline);

		ImGui::NewLine();
		CenterText("For support check the troubleshooting page:");
		CenterImage(url, 0.8f);
		CenterText("github.com/exelix11/SysDVR/wiki/Troubleshooting");

		ImGui::NewLine();
		CenterText("Press + to quit");

		ImGui::End();
		UI::EndFrame();
		Platform::Sleep(1 / 30.0f * 1000);
	}
}

static void FatalError(const char* message, Result rc)
{
	std::string errorMsg;
	Image::Img url(ASSET("troubleshooting.png"));

	if (rc) 
		errorMsg = "Error code: " + std::to_string(2000 + R_MODULE(rc)) + "-" + std::to_string(R_DESCRIPTION(rc));
	else
		errorMsg = "";

	FatalError(message, errorMsg);
}

constexpr float ModeButtonW = 1280 * 0.7f;
static bool ModeButton(std::string_view title, std::string_view description, Image::Img& image, bool Active, bool OnBoot)
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

	ImVec2 size = ImGui::CalcItemSize({ ModeButtonW, 0},
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
	ImGui::RenderTextWrapped(contentStart + style.FramePadding + ImVec2{image.Size().x, label_size.y}, description.data(), description.data() + description.size(), ModeButtonW - image.Size().x - style.FramePadding.x * 2);
	
	const ImVec2 imagePos = {contentStart.x , contentStart.y + style.FramePadding.y + label_size.y};
	window->DrawList->AddImage(image, imagePos, imagePos + image.Size());
	
	if (Active || OnBoot) 
	{
		ImVec2 pos = bb.GetTR();
		if (Active)
		{
			const ImVec2 sz = ImGui::CalcTextSize("Enabled") + ImVec2{6, 0};
			pos.x -= sz.x;
			window->DrawList->AddRectFilled(pos, pos + sz, 0xFF008800);
			ImGui::RenderText(pos + ImVec2{3,0}, "Enabled");
		}
		if (OnBoot)
		{
			const ImVec2 sz = ImGui::CalcTextSize("On boot") + ImVec2{6, 0};
			pos.x -= sz.x;
			window->DrawList->AddRectFilled(pos, pos + sz, 0xFF000088);
			ImGui::RenderText(pos + ImVec2{ 3,0 }, "On boot");
		}
	}

	IMGUI_TEST_ENGINE_ITEM_INFO(id, label, window->DC.LastItemStatusFlags);
	return pressed;
}

template <size_t N, typename T>
static inline int ImGuiCenterButtons(T(&& buttons)[N])
{
	const auto str = [&buttons](size_t i) -> const char*
	{
		if constexpr (std::is_same<T, std::string>())
			return buttons[i].c_str();
		else if constexpr (std::is_same<T, const char*>())
			return buttons[i];
	};

	const ImGuiStyle& style = GImGui->Style;
	auto win = ImGui::GetWindowWidth();
	float TotX = 0;
	for (size_t i = 0; i < N; i++)
	{
		auto sz = ImGui::CalcTextSize(str(i), nullptr, false, win);
		TotX += ImGui::CalcItemSize({}, sz.x + style.FramePadding.x * 2.0f, sz.y + style.FramePadding.y * 2.0f).x;
	}
	ImGui::SetCursorPosX((win / 2 - TotX / 2) - (N - 1) * style.FramePadding.x * 2);
	int res = -1;
	for (size_t i = 0; i < N; i++)
	{
		if (ImGui::Button(str(i)))
			res = i;
		if (i != N - 1)
			ImGui::SameLine();
	}
	return res;
}

enum class Scene{
	ModeSelect,
	Guide,
	Error,
	Quit
};

static u32 CurrentMode;
static u32 BootMode;

std::string RtspDescription = "Stream directly to any video player application via RTSP.\n"
							  "Once you enable and apply it open rtsp://{}:6666/ with any video player like mpv or vlc.\n"
							  "This mode doesn't require SysDVR-Client on your PC.\n"
							  "For network modes it's recommended to use a LAN adapter.";

const std::string TcpDescription = "Stream to the SysDVR-Client application via network. This is more stable than the simple network mode.\n"
								   "To setup SysDVR-Client on your pc refer to the guide on Github\n"
								   "For network modes it's recommended to use a LAN adapter.";

const std::string UsbDescription = "Use this mode to stream to the SysDVR-Client application via USB.\n"
								   "To setup SysDVR-Client on your pc refer to the guide on Github";

static Scene ModeSelection()
{	
	const auto SetMode = [](u32 mode) -> bool 
	{
		Result rc = SysDvrSetMode(mode);

		if (R_FAILED(rc))
		{
			// This is called while drawing, finish rendering the frame
			ImGui::End();
			UI::EndFrame();

			FatalError("Couldn't change mode", rc);
			return false;
		}

		CurrentMode = mode;
		return true;
	};

	SetupMainWindow("Select mode", .6f);

	ImGui::SetCursorPosX(1280 / 2 - ModeButtonW / 2);
	if (ModeButton("Simple network mode", RtspDescription, ModeRtsp, CurrentMode == TYPE_MODE_RTSP, BootMode == TYPE_MODE_RTSP))
		if (!SetMode(TYPE_MODE_RTSP))
			return Scene::Error;

	ImGui::SetCursorPosX(1280 / 2 - ModeButtonW / 2);
	if (ModeButton("TCP Bridge", TcpDescription, ModeTcp, CurrentMode == TYPE_MODE_TCP, BootMode == TYPE_MODE_TCP))
		if (!SetMode(TYPE_MODE_TCP))
			return Scene::Error;

	ImGui::SetCursorPosX(1280 / 2 - ModeButtonW / 2);
	if (ModeButton("USB", UsbDescription, ModeUsb, CurrentMode == TYPE_MODE_USB, BootMode == TYPE_MODE_USB))
		if (!SetMode(TYPE_MODE_USB))
			return Scene::Error;

	ImGui::SetCursorPosX(1280 / 2 - ModeButtonW / 2);
	if (ImGui::Button("Stop streaming", { ModeButtonW , 0}))
		if (!SetMode(TYPE_MODE_NULL))
			return Scene::Error;

	ImGui::NewLine();
	Scene result = Scene::ModeSelect;
	switch (ImGuiCenterButtons({"Guide", "Set current mode as default on boot", "Save and exit"}))
	{
		case 0:
			result = Scene::Guide;
			break;
		case 1:
			if (!SetDefaultBootMode(CurrentMode))
			{
				// This is called while drawing, finish rendering the frame
				ImGui::End();
				UI::EndFrame();
				FatalError("Couldn't set boot mode", "Try checking your SD card for corruption");
				return Scene::Error;
			}
			BootMode = CurrentMode;
			break;
		case 2:
			result = Scene::Quit;
			break;
		default:
			break;
	}

	ImGui::End();
	return result;
}

static Scene ShowGuide()
{	
	SetupMainWindow("Guide", .6f);
	ImGui::NewLine();

	CenterText("The guide is hosted on Github:");
	CenterImage(Guide);
	CenterText("github.com/exelix11/SysDVR/wiki/");

	ImGui::NewLine();
	
	Scene result = Scene::Guide;
	if (ImGuiCenterButtons({"    Back   "}) == 0)
		result = Scene::ModeSelect;

	ImGui::End();

	return result;
}

int main(int argc, char* argv[])
{
	Platform::Init();

	if (!UI::Init())
	{
		Platform::Exit();
		return -1;
	}
	Platform::AfterInit();

	BootMode = GetBootMode();

	SysDVRLogo = Image::Img(ASSET("logo.png"));
	ModeRtsp = Image::Img(ASSET("ModeRtsp.png"));
	ModeTcp = Image::Img(ASSET("ModeTcp.png"));
	ModeUsb = Image::Img(ASSET("ModeUsb.png"));

	Guide = Image::Img(ASSET("guide.png"));

	char hostname[128] = "<console IP address>";
#ifdef __SWITCH__	
	if (gethostname(hostname, sizeof(hostname)) || !strcmp(hostname, "1.0.0.127"))
		strcpy(hostname, "<console IP address>");
#endif
	RtspDescription.replace(RtspDescription.find("{}"), 2, hostname);

	Result rc = ConnectToSysmodule();

	if (R_FAILED(rc))
		FatalError("Couldn't connect to SysDVR", rc);
	
	u32 version;
	rc = SysDvrGetVersion(&version);
	if (R_FAILED(rc))
		FatalError("Couldn't get the SysDVR version", rc);

	if (version > SYSDVR_VERSION)
		FatalError("You're using a newer version of SysDVR", "Please download the latest settings app from github");
	else if (version < SYSDVR_VERSION)
		FatalError("You're using an outdated version of SysDVR","Please download the latest version from github");

	rc = SysDvrGetMode(&CurrentMode);
	if (R_FAILED(rc))
		FatalError("Couldn't get the SysDVR mode", rc);

	if (CurrentMode == TYPE_MODE_SWITCHING)
		FatalError("SysDVR is already switching modes", "Enter a game to complete the operation");
	else if (CurrentMode == TYPE_MODE_ERROR)
		FatalError("Couldn't get the current SysDVR mode", "Try rebooting your console");

	Scene curMode = Scene::ModeSelect;
	while (App::MainLoop())
	{
		Platform::GetInputs();
		ImguiBindController();
		Platform::ImguiBindings();

		UI::StartFrame();
		
		switch (curMode)
		{
		case Scene::ModeSelect:
			curMode = ModeSelection();
			break;
		case Scene::Guide:
			curMode = ShowGuide();
			break;
		case Scene::Error:
			goto Quit_Immediate;
		default:
			App::SetShouldClose();
			break;
		}

		UI::EndFrame();
		Platform::Sleep(1 / 30.0f * 1000);
	}

Quit_Immediate:

	SysDvrClose();
	UI::Exit();
	Platform::Exit();
	return EXIT_SUCCESS;
}
