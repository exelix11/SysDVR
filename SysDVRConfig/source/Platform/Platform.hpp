#pragma once
#define GLFW_INCLUDE_NONE
#include <GLFW/glfw3.h>
#include <string_view>

extern GLFWgamepadstate g_gamepad;

namespace Platform{	
	void Init();
	void Exit();
	void AfterInit();
	void GetInputs();
	void ImguiBindings();
	void Sleep(float time);
	void Reboot();

	std::string_view GetSystemLanguage();
}