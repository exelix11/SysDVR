#pragma once
#include <string>
#include <vector>
#include "imgui/imgui.h"
#include "../Platform/PlatformFs.hpp"
#include "UI.hpp"

namespace Image
{
	struct LoadResult
	{
		int x, y;
		GLuint id;
	};

	LoadResult Load(const std::vector<u8>& buf);
	LoadResult Load(const std::string& path);

	void Free(GLuint img);

	class Img
	{
	public:
		Img();
		Img(const std::vector<u8>& buf);
		Img(const std::string& path);
		Img(Img&& other);
		Img& operator=(Img&& other);
		~Img();

		Img(const Img& other) = delete;
		Img& operator=(const Img& other) = delete;

		constexpr GLuint ID() const;
		ImVec2 Size() const;

		constexpr operator GLuint() const;
		operator ImTextureID() const;
	protected:
		constexpr Img(LoadResult res);
		constexpr Img(GLuint id, int x, int y);

	private:
		GLuint id;
		float x, y;
	};
}