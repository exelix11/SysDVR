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
	static inline void CreateDirectory(const std::string& path) { mkdir(path.c_str(), 0777); }
	static inline void DeleteDirectory(const std::string& path) { rmdir(path.c_str()); }
}