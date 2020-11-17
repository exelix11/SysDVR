#pragma once

#define GLFW_INCLUDE_NONE
#include <GLFW/glfw3.h>
#include "glad.h"
#define IMGUI_DEFINE_MATH_OPERATORS
#include "imgui/imgui.h"
#include "imgui/imgui_internal.h"

namespace UI {
	constexpr uint32_t WindowWidth = 1280;
	constexpr uint32_t WindowHeight = 720;
	constexpr const char* WindowTitle = "SwitchApplication";
	
	extern GLFWwindow* MainWindow;
	
	extern float WRatio;
	extern float HRatio;

	bool Init();
	void Exit();
		
	void StartFrame();
	void EndFrame();
}

namespace App {
	bool MainLoop();
	void SetShouldClose();
}