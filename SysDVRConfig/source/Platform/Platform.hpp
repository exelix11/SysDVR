#pragma once
#define GLFW_INCLUDE_NONE
#include <GLFW/glfw3.h>

extern GLFWgamepadstate g_gamepad;

namespace Platform{	
	void Init();
	void Exit();
	void AfterInit();
	void GetInputs();
	void ImguiBindings();
	void Sleep(float time);
}