#include "Scenes.hpp"
#include "Common.hpp"

#include "../Platform/fs.hpp"

#include <curl/curl.h>

#include "nlohmann/json.hpp"

#include "zip/zip.h"

using namespace std::string_literals;

#define DVRPATCHES_DIR "/atmosphere/exefs_patches/am/"
#define DVRPATCHES_VERSION_FILE DVRPATCHES_DIR "version"

namespace {
	enum PatchStatus {
		Insstalled,
		VersionUnknown,
		NotInstalled
	};

	PatchStatus patchStatus;
	std::string patchVersion;
	
	void (*scheduledAction)();
	int scheduledCounter;

	bool showRebootWarning;
	std::string statusMessage;

	std::string updateFoundUrl;
	std::string updateInfo;
	std::string updateVersion;

	void UpdatePatchesStatus() 
	{
		if (fs::Exists(SDMC DVRPATCHES_VERSION_FILE))
		{
			auto file = fs::OpenFile(SDMC DVRPATCHES_VERSION_FILE);
			std::string version(file.begin(), file.end());
			patchVersion = version;
			patchStatus = PatchStatus::Insstalled;
		}
		else if (fs::Exists(SDMC DVRPATCHES_DIR))
		{
			patchStatus = PatchStatus::VersionUnknown;
		}
		else
		{
			patchStatus = PatchStatus::NotInstalled;
		}
	}
	
	void UninstallPatches() 
	{
		statusMessage = "";
		
		fs::DeleteDir(SDMC DVRPATCHES_DIR);
		UpdatePatchesStatus();
		showRebootWarning = true;
	}
	
	CURL* curl;

	bool InitCurl() {
		if (curl)
			return true;

		curl = curl_easy_init();
		if (!curl)
		{
			statusMessage = "Curl initialization failed";
			return false;
		}
		
		return true;
	}

	void DeinitCurl() {
		if (curl)
		{
			curl_easy_cleanup(curl);
			curl = nullptr;
		}
	}

	std::vector<u8> HttpGET(const std::string& url)
	{
		if (!InitCurl())
			return {};

		std::vector<u8> data;

		curl_easy_setopt(curl, CURLOPT_URL, url.c_str());
		curl_easy_setopt(curl, CURLOPT_FOLLOWLOCATION, 1);
		curl_easy_setopt(curl, CURLOPT_USERAGENT, "SysDVR Settings");
		
		curl_easy_setopt(curl, CURLOPT_WRITEFUNCTION, +[](void* contents, size_t size, size_t nmemb, void* userp) 
			{
				auto& data = *(std::vector<u8>*)userp;
				size_t realsize = size * nmemb;
				data.insert(data.end(), (u8*)contents, (u8*)contents + realsize);
				return realsize;
			}
		);

		curl_easy_setopt(curl, CURLOPT_WRITEDATA, &data);

		CURLcode res = curl_easy_perform(curl);
		if (res != CURLE_OK)
		{
			statusMessage = "Curl GET failed: " + std::string(curl_easy_strerror(res));
			return {};
		}
		else if (data.size() == 0)
		{
			statusMessage = "No data received";
		}
		
		return data;
	}

	void SearchForUpdate() {
		updateFoundUrl = "";
		
		auto releaseInfo = HttpGET("https://api.github.com/repos/exelix11/dvr-patches/releases/latest");
		if (releaseInfo.size() == 0)
			return;
		
		auto tagInfo = HttpGET("https://api.github.com/repos/exelix11/dvr-patches/tags");
		if (tagInfo.size() == 0)
			return;

		auto releaseDoc = nlohmann::json::parse(releaseInfo.begin(), releaseInfo.end());

		auto& tagName = releaseDoc["tag_name"];
		if (tagName.is_null())
		{
			statusMessage = "Failed to parse release info";
			return;
		}

		std::string commitName = "";
		
		{
			auto tagsDoc = nlohmann::json::parse(tagInfo.begin(), tagInfo.end());

			if (!tagsDoc.is_array())
			{
				statusMessage = "Failed to parse tags info";
				return;
			}

			for (auto& tag : tagsDoc)
			{
				if (tag["name"] == tagName)
				{
					commitName = (std::string) tag["commit"]["sha"];
					break;
				}
			}
		}

		if (commitName == "")
		{
			statusMessage = "Failed to find commit for tag "s + (std::string) tagName;
			return;
		}

		if (patchStatus == PatchStatus::Insstalled && patchVersion == commitName)
		{
			statusMessage = "You're already using latest version of dvr-patches.";
			return;
		}

		auto& releaseUrl = releaseDoc["assets"][0]["browser_download_url"];
		if (releaseUrl.is_null())
		{
			statusMessage = "Failed to parse release info";
			return;
		}

		updateFoundUrl = (std::string)releaseUrl;
		updateVersion = commitName;
		updateInfo = "New version of dvr-patches available: "s + (std::string)tagName + "\n" + (std::string)releaseDoc["body"];
	}

	void DownloadRelease() {
		if (updateFoundUrl.size() == 0)
		{
			statusMessage = "No update source found";
			return;
		}

		auto data = HttpGET(updateFoundUrl);
		if (data.size() == 0)
			return;

		zip_t* zip = zip_stream_open((const char*)data.data(), data.size(), ZIP_DEFAULT_COMPRESSION_LEVEL, 'r');
		if (!zip)
		{
			statusMessage = "Failed to open zip archive";
			return;
		}

		fs::CreateDir(SDMC DVRPATCHES_DIR);

		int entryCnt = zip_entries_total(zip);
		for (int i = 0; i < entryCnt; ++i) {
			zip_entry_openbyindex(zip, i);
			if (!zip_entry_isdir(zip))
			{
				const char* name = zip_entry_name(zip);
				unsigned long long size = zip_entry_size(zip);
				
				std::vector<u8> entryData(size);
				zip_entry_noallocread(zip, entryData.data(), size);
				
				fs::WriteFile(std::string(SDMC DVRPATCHES_DIR) + std::filesystem::path(name).filename().string(), entryData);
			}
			zip_entry_close(zip);
		}
		
		zip_stream_close(zip);		
		
		fs::WriteFile(SDMC DVRPATCHES_VERSION_FILE, { updateVersion.begin(), updateVersion.end() });

		updateFoundUrl = "";
		updateInfo = "";

		UpdatePatchesStatus();
		showRebootWarning = true;
		statusMessage = "Update downloaded.";
	}
}

namespace scenes {
	void InitDvrPatches() 
	{
		UpdatePatchesStatus();
	}
	
	void DeinitDvrPatches()
	{
		DeinitCurl();
	}

	void DvrPatches()
	{
		constexpr auto paddedFrameWidth = UI::WindowWidth * 4 / 5;
		ImGui::PushStyleVar(ImGuiStyleVar_WindowPadding, ImVec2((UI::WindowWidth - paddedFrameWidth) / 2, 0));
		SetupMainWindow("Dvr-patches");
		
		ImGui::SetCursorPosY(30);
		CenterText("Dvr-patches manager");
		ImGui::NewLine();

		if (scheduledAction)
		{
			ImGui::NewLine();
			ImGui::NewLine();
			ImGui::NewLine();
			ImGui::NewLine();
			CenterText("Loading...");
			
			if (scheduledCounter++ > 5)
			{
				scheduledAction();
				scheduledAction = nullptr;
				scheduledCounter = 0;
			}
		}
		else {

			ImGui::TextWrapped(
				"Dvr-patches are system patches that allow to stream most incompatible games with SysDVR.\n"
				"Dvr-patches are not enabled by default as they may cause issues with certain games, you can read more on the GitHub repository https://github.com/exelix11/dvr-patches\n"
				"From this page you can download latest version of dvr-patches from github or delete them in case you're facing issues."
			);

			ImGui::NewLine();
			ImGui::Text("Dvr-patches status: ");
			ImGui::SameLine();

			if (patchStatus == PatchStatus::NotInstalled)
			{
				ImGui::TextColored(ImVec4(1.0f, 0.0f, 0.0f, 1.0f), "not installed");

			}
			else if (patchStatus == PatchStatus::VersionUnknown)
			{
				ImGui::TextColored(ImVec4(1.0f, 1.0f, 0.6f, 1.0f), "installed, version unknown");
				ImGui::Text("Sdcard path: " DVRPATCHES_DIR "     ");
				ImGui::SameLine();
				if (ImGui::Button("Uninstall"))
				{
					scheduledAction = UninstallPatches;
				}
			}
			else
			{
				ImGui::TextColored(ImVec4(0.0f, 1.0f, 0.0f, 1.0f), "commit %s installed", patchVersion.c_str());
				ImGui::Text("Sdcard path: " DVRPATCHES_DIR "     ");
				ImGui::SameLine();
				if (ImGui::Button("Uninstall"))
				{
					scheduledAction = UninstallPatches;
				}
			}
			ImGui::NewLine();

			if (statusMessage.size() > 0)
			{
				ImGui::TextColored(ImVec4(1.0f, 1.0f, 0.6f, 1.0f), "%s", statusMessage.c_str());
				ImGui::NewLine();
			}

			if (updateFoundUrl.size() > 0)
			{
				ImGui::TextWrapped("%s", updateInfo.c_str());
				if (ImGuiCenterButtons({ "Download now" }) == 0)
				{
					scheduledAction = DownloadRelease;
				}
				ImGui::NewLine();
			}

			if (showRebootWarning)
			{
				ImGui::Text("To apply the changes you need to reboot your console    ");
				ImGui::SameLine();
				if (ImGui::Button("Reboot now"))
				{
					Platform::Reboot();
				}

				ImGui::NewLine();
			}

			ImGui::NewLine();

			switch (ImGuiCenterButtons({ "Go back", "Search for latest patches on GitHub" }))
			{
			case 0:
				app::SetNextScene(Scene::ModeSelect);
				break;
			case 1:
				scheduledAction = SearchForUpdate;
				break;
			}
		}
		
		ImGui::End();
		ImGui::PopStyleVar();
	}
}