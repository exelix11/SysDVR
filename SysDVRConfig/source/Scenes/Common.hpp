#pragma once

#include <string>
#include <cstdint>
#include <string_view>

#include "../UI/UI.hpp"
#include "../UI/Image.hpp"
#include "../Platform/Platform.hpp"

// Shared from main.coo
extern Image::Img SysDVRLogo;

static inline bool AnyNavButtonPressed()
{
	return  g_gamepad.buttons[GLFW_GAMEPAD_BUTTON_DPAD_DOWN] || g_gamepad.buttons[GLFW_GAMEPAD_BUTTON_DPAD_UP] ||
		g_gamepad.buttons[GLFW_GAMEPAD_BUTTON_DPAD_LEFT] || g_gamepad.buttons[GLFW_GAMEPAD_BUTTON_DPAD_RIGHT];
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

static void SetupMainWindow(const char* name, float padding = 0)
{
	ImGui::SetNextWindowSize({ UI::WindowWidth - padding, UI::WindowHeight });
	ImGui::SetNextWindowPos({ (UI::WindowWidth - (UI::WindowWidth - padding)) / 2, 0});
	
	ImGui::Begin(name, 0, ImGuiWindowFlags_NoDecoration | ImGuiWindowFlags_NoMove | ImGuiWindowFlags_NoResize);
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
