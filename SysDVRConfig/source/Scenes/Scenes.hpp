#pragma once

#include <cstdint>
#include <string_view>

enum class Scene {
	ModeSelect,
	Guide,
	FatalError,
	DvrPatches,
	NoConnection,
	DevScene
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
	
	void NoConnection();
	
	void InitDevScene();
	void DevTestScene();
}

// from main.cpp
namespace app {
	void FatalError(std::string_view message, std::string_view secondline);
	void FatalErrorWithErrorCode(std::string_view message, uint32_t rc);
	
	void SetNextScene(Scene s);
	void ReturnToPreviousScene(); 

	void SetWaitOnExit(bool shouldWait);
	void RequestExit();
}