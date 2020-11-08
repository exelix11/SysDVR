#include "fs.hpp"

std::vector<u8> fs::OpenFile(const std::string& name)
{
	FILE* f = fopen(name.c_str(), "rb");
	if (!f)
		throw std::runtime_error("Opening file " + name + " failed !\n");

	fseek(f, 0, SEEK_END);
	size_t len = 0;
	{
		auto fsz = ftell(f);
		if (fsz < 0)
			throw std::runtime_error("Reading file size for " + name + " failed !\n");
		len = fsz;
	}
	rewind(f);

	std::vector<u8> coll(len);
	if (fread(coll.data(), 1, len, f) != len)
		throw std::runtime_error("Reading from file " + name + " failed !\n");

	fclose(f);
	return coll;
}

void fs::WriteFile(const std::string& name, const std::vector<u8>& data)
{
	if (std::filesystem::exists(name))
		remove(name.c_str());

	FILE* f = fopen(name.c_str(), "wb");
	if (!f)
		throw std::runtime_error("Saving file " + name + "failed !");

	fwrite(data.data(), 1, data.size(), f);
	fflush(f);
	fclose(f);
}