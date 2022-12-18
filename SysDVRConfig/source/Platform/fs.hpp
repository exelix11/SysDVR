#include <cstdio>
#include <vector>
#include <string>
#include <filesystem>

#include "PlatformFs.hpp"

namespace fs {
	std::vector<u8> OpenFile(const std::string& name);
	void WriteFile(const std::string& name, const std::vector<u8>& data);

	static inline bool Exists(const std::string& name) { return std::filesystem::exists(name); }
	static inline void Delete(const std::string& path) { unlink(path.c_str()); }
	static inline void CreateDir(const std::string& path) { std::filesystem::create_directories(path); }
	static inline void DeleteDir(const std::string& path) { std::filesystem::remove_all(path); }
}