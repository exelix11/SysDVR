#ifndef __SWITCH__

#include "Platform.hpp"
#include <iostream>
#include "../UI/UI.hpp"

GLFWgamepadstate g_gamepad;

static void windowKeyCallback(GLFWwindow* window, int key, int scancode, int action, int mods)
{
	ImGui::GetIO().KeysDown[key] = action;
	if (action == GLFW_PRESS && key >= 32 && key <= 126)
		ImGui::GetIO().AddInputCharacter(key);

	// Check for toggle-fullscreen combo
	if (key == GLFW_KEY_ENTER && mods == GLFW_MOD_ALT && action == GLFW_PRESS)
	{
		static int saved_x, saved_y, saved_width, saved_height;

		if (!glfwGetWindowMonitor(window))
		{
			// Back up window position/size
			glfwGetWindowPos(window, &saved_x, &saved_y);
			glfwGetWindowSize(window, &saved_width, &saved_height);

			// Switch to fullscreen mode
			GLFWmonitor* monitor = glfwGetPrimaryMonitor();
			const GLFWvidmode* mode = glfwGetVideoMode(monitor);
			glfwSetWindowMonitor(window, monitor, 0, 0, mode->width, mode->height, mode->refreshRate);
		}
		else
		{
			// Switch back to windowed mode
			glfwSetWindowMonitor(window, nullptr, saved_x, saved_y, saved_width, saved_height, GLFW_DONT_CARE);
		}
	}
}

void Platform::AfterInit()
{
	glfwSetKeyCallback(UI::MainWindow, windowKeyCallback);
	ImGuiIO& io = ImGui::GetIO();
	io.KeyMap[ImGuiKey_Delete] = GLFW_KEY_DELETE;
	io.KeyMap[ImGuiKey_Backspace] = GLFW_KEY_BACKSPACE;
}

void Platform::GetInputs()
{
	if (glfwGetGamepadState(GLFW_JOYSTICK_1, &g_gamepad)) return;
	
	g_gamepad.buttons[GLFW_GAMEPAD_BUTTON_DPAD_LEFT] = glfwGetKey(UI::MainWindow, GLFW_KEY_LEFT);
	g_gamepad.buttons[GLFW_GAMEPAD_BUTTON_DPAD_RIGHT] = glfwGetKey(UI::MainWindow, GLFW_KEY_RIGHT);
	g_gamepad.buttons[GLFW_GAMEPAD_BUTTON_DPAD_UP] = glfwGetKey(UI::MainWindow, GLFW_KEY_UP);
	g_gamepad.buttons[GLFW_GAMEPAD_BUTTON_DPAD_DOWN] = glfwGetKey(UI::MainWindow, GLFW_KEY_DOWN);
	g_gamepad.buttons[GLFW_GAMEPAD_BUTTON_START] = glfwGetKey(UI::MainWindow, GLFW_KEY_ESCAPE);

	g_gamepad.buttons[GLFW_GAMEPAD_BUTTON_A] = glfwGetKey(UI::MainWindow, GLFW_KEY_A) || glfwGetKey(UI::MainWindow, GLFW_KEY_ENTER);
	g_gamepad.buttons[GLFW_GAMEPAD_BUTTON_B] = glfwGetKey(UI::MainWindow, GLFW_KEY_S);
}

void Platform::ImguiBindings()
{
	double mouseX, mouseY;
	glfwGetCursorPos(UI::MainWindow, &mouseX, &mouseY);
	ImGuiIO &io = ImGui::GetIO();
	io.MousePos = ImVec2((float)mouseX / UI::WRatio, (float)mouseY / UI::HRatio);
	io.MouseDown[0] = glfwGetMouseButton(UI::MainWindow, 0) == GLFW_PRESS;
}

void Platform::Init() {}
void Platform::Exit() {}

void Platform::Sleep(float time)
{
	_sleep((unsigned long)time);
}

void Platform::Reboot()
{
	exit(0);
}

#endif