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
	
	extern GLFWwindow* MainWindow;
	
	extern float WRatio;
	extern float HRatio;

	extern ImFont* font20;
	extern ImFont* font40;

	bool Init();
	void Exit();
		
	void StartFrame();
	void EndFrame();

	void DecodeFont(uint8_t* data, uint32_t len);
}

namespace Glfw {
	bool MainLoop();
	void SetShouldClose();
}