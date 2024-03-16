#include "Scenes.hpp"
#include "Common.hpp"

#include <curl/curl.h>

#include "../translaton.hpp"
#include "../Platform/fs.hpp"
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

	std::string sdCartPathlabel;

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
			statusMessage = Strings::Patches.CurlError;
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
			statusMessage = Strings::Patches.CurlGETFailed + ": " + std::string(curl_easy_strerror(res));
			return {};
		}
		else if (data.size() == 0)
		{
			statusMessage = Strings::Patches.CurlNoData;
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
			statusMessage = Strings::Patches.ParseReleaseFailure;
			return;
		}

		std::string commitName = "";
		
		{
			auto tagsDoc = nlohmann::json::parse(tagInfo.begin(), tagInfo.end());

			if (!tagsDoc.is_array())
			{
				statusMessage = Strings::Patches.ParseTagFailure;
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
			statusMessage = Strings::Patches.ParseTagCommitFailure + ": " + (std::string) tagName;
			return;
		}

		if (patchStatus == PatchStatus::Insstalled && patchVersion == commitName)
		{
			statusMessage = Strings::Patches.LatestVer;
			return;
		}

		auto& releaseUrl = releaseDoc["assets"][0]["browser_download_url"];
		if (releaseUrl.is_null())
		{
			statusMessage = Strings::Patches.ParseDownloadFailure;
			return;
		}

		updateFoundUrl = (std::string)releaseUrl;
		updateVersion = commitName;
		updateInfo = Strings::Patches.NewVerAvail + " "s + (std::string)tagName + "\n" + (std::string)releaseDoc["body"];
	}

	void DownloadRelease() {
		if (updateFoundUrl.size() == 0)
		{
			statusMessage = Strings::Patches.NoLinkFound;
			return;
		}

		auto data = HttpGET(updateFoundUrl);
		if (data.size() == 0)
			return;

		zip_t* zip = zip_stream_open((const char*)data.data(), data.size(), ZIP_DEFAULT_COMPRESSION_LEVEL, 'r');
		if (!zip)
		{
			statusMessage = Strings::Patches.ZipExtractFail;
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
		statusMessage = Strings::Patches.DownloadOk;
	}
}

namespace scenes {
	void InitDvrPatches() 
	{
		sdCartPathlabel = Strings::Patches.SdcardPath + " " DVRPATCHES_DIR "     ";
		UpdatePatchesStatus();
	}
	
	void DeinitDvrPatches()
	{
		DeinitCurl();
	}

	void DvrPatches()
	{
		SetupMainWindow("Dvr-patches", UI::DefaultFramePadding);
		
		ImGui::SetCursorPosY(30);
		
		UI::BigFont();
		CenterText(Strings::Patches.Title);
		UI::PopFont();
	
		ImGui::NewLine();

		if (scheduledAction)
		{
			ImGui::NewLine();
			ImGui::NewLine();
			ImGui::NewLine();
			ImGui::NewLine();
			CenterText(Strings::Patches.Loading);
			
			if (scheduledCounter++ > 5)
			{
				scheduledAction();
				scheduledAction = nullptr;
				scheduledCounter = 0;
			}
		}
		else {

			ImGui::TextWrapped(Strings::Patches.Description.c_str());

			ImGui::NewLine();
			ImGui::Text(Strings::Patches.Status.c_str());
			ImGui::SameLine();

			if (patchStatus == PatchStatus::NotInstalled)
			{
				ImGui::TextColored(ImVec4(1.0f, 0.0f, 0.0f, 1.0f), Strings::Patches.StatusNotInstalled.c_str());

			}
			else if (patchStatus == PatchStatus::VersionUnknown)
			{
				ImGui::TextColored(ImVec4(1.0f, 1.0f, 0.6f, 1.0f), Strings::Patches.StatusUnknownVersion.c_str());
				ImGui::Text(sdCartPathlabel.c_str());
				ImGui::SameLine();
				
				if (ImGui::Button(Strings::Patches.UninstallButton.c_str()))
					scheduledAction = UninstallPatches;
			}
			else
			{
				ImGui::TextColored(ImVec4(0.0f, 1.0f, 0.0f, 1.0f), Strings::Patches.StatusInstalled.c_str(), patchVersion.c_str());
				ImGui::Text(sdCartPathlabel.c_str());
				ImGui::SameLine();

				if (ImGui::Button(Strings::Patches.UninstallButton.c_str()))
					scheduledAction = UninstallPatches;
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
				if (ImGuiCenterButtons<std::string_view>({ Strings::Patches.DownloadButton }) == 0)
				{
					scheduledAction = DownloadRelease;
				}
				ImGui::NewLine();
			}

			if (showRebootWarning)
			{
				ImGui::Text(Strings::Patches.RebootWarning.c_str());
				ImGui::SameLine();
				if (ImGui::Button(Strings::Patches.RebootButton.c_str()))
				{
					Platform::Reboot();
				}

				ImGui::NewLine();
			}

			ImGui::NewLine();

			switch (ImGuiCenterButtons<std::string_view>({ Strings::Patches.BackButton, Strings::Patches.SearchLatestButton }))
			{
			case 0:
				app::ReturnToPreviousScene();
				break;
			case 1:
				scheduledAction = SearchForUpdate;
				break;
			}
		}
		
		ImGui::End();
	}
}