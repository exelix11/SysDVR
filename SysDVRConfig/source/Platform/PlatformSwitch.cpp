#ifdef __SWITCH__

#include "Platform.hpp"
#include <iostream>
#include <switch.h>
#include <unistd.h>
#include "../UI/UI.hpp"

#ifdef __NXLINK_ENABLE__
static int nxlink_sock = -1;
#endif

GLFWgamepadstate g_gamepad;

void Platform::Init() 
{
	romfsInit();
	socketInitializeDefault();
	hidInitializeTouchScreen();
#ifdef __NXLINK_ENABLE__
	nxlink_sock = nxlinkStdio();
#endif
}

void Platform::Exit()
{
#ifdef __NXLINK_ENABLE__
	if (nxlink_sock != -1)
		close(nxlink_sock);
#endif
	socketExit();
	romfsExit();
}

void Platform::AfterInit()
{

}

void Platform::GetInputs()
{
	if (!glfwGetGamepadState(GLFW_JOYSTICK_1, &g_gamepad))
		std::cout << "Error reading from gamepad";
}

void Platform::ImguiBindings()
{
	ImGuiIO &io = ImGui::GetIO();
	HidTouchScreenState state;
	if (hidGetTouchScreenStates(&state, 1) && state.count == 1)
	{
		io.MousePos = ImVec2(state.touches[0].x, state.touches[0].y);
		io.MouseDown[0] = true;
	}
	else io.MouseDown[0] = false;
}

void Platform::Sleep(float time)
{
	usleep(time * 1000);
}

void Platform::Reboot()
{
	spsmInitialize();
	spsmShutdown(true);
}

std::string_view Platform::GetSystemLanguage()
{
	SetLanguage Language = SetLanguage_ENUS;
	u64 s_textLanguageCode = 0;

	Result rc = setInitialize();
	if (R_SUCCEEDED(rc)) rc = setGetSystemLanguage(&s_textLanguageCode);
	if (R_SUCCEEDED(rc)) rc = setMakeLanguage(s_textLanguageCode, &Language);
	setExit();

	if (R_FAILED(rc))
		return "SetLanguage_ENUS";

#define LANG_MATCH(x) case x: return #x
	switch (Language)
	{
		LANG_MATCH(SetLanguage_JA);
		LANG_MATCH(SetLanguage_ENUS);
		LANG_MATCH(SetLanguage_FR);
		LANG_MATCH(SetLanguage_DE);
		LANG_MATCH(SetLanguage_IT);
		LANG_MATCH(SetLanguage_ES);
		LANG_MATCH(SetLanguage_ZHCN);
		LANG_MATCH(SetLanguage_KO);
		LANG_MATCH(SetLanguage_NL);
		LANG_MATCH(SetLanguage_PT);
		LANG_MATCH(SetLanguage_RU);
		LANG_MATCH(SetLanguage_ZHTW);
		LANG_MATCH(SetLanguage_ENGB);
		LANG_MATCH(SetLanguage_FRCA);
		LANG_MATCH(SetLanguage_ES419);
		LANG_MATCH(SetLanguage_ZHHANS);
		LANG_MATCH(SetLanguage_ZHHANT);
		LANG_MATCH(SetLanguage_PTBR);
		default: return "SetLanguage_ENUS";
	}	
#undef LANG_MATCH
}

#endif