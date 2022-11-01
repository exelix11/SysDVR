#pragma once

#include <cstdint>
#include <string_view>

enum class Scene {
	ModeSelect,
	Guide,
	FatalError,
	DvrPatches
};

// Each implemented in its own scenes cpp file
namespace scenes {
	void InitModeSelect();
	void ModeSelect();

	void InitGuide();
	void Guide();

	void InitDvrPatches();
	void DeinitDvrPatches();
	void DvrPatches();

	void InitFatalError();
	void FatalError();
}

// from main.cpp
namespace app {
	void FatalError(std::string_view message, std::string_view secondline);
	void FatalErrorWithErrorCode(std::string_view message, uint32_t rc);
	void SetNextScene(Scene s);
	void RequestExit();
}