#include "Image.hpp"
#include "../Platform/fs.hpp"
#include "UI.hpp"
#include "stb_image.h"

Image::LoadResult Image::Load(const std::vector<u8>& buf)
{
	int x,y, ch;
	auto data = stbi_load_from_memory(buf.data(), buf.size(), &x, &y, &ch, 4);

	if (!data)
		return {0,0,0};

	GLuint id;
	glGenTextures(1, &id);
	glBindTexture(GL_TEXTURE_2D, id);

	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);

	glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, x, y, 0, GL_RGBA, GL_UNSIGNED_BYTE, data);

	glBindTexture(GL_TEXTURE_2D, 0);

	stbi_image_free(data);

	return {x,y,id};
}

Image::LoadResult Image::Load(const std::string& path)
{
	return Image::Load(fs::OpenFile(path));
}

void Image::Free(GLuint img)
{
	glDeleteTextures(1, &img);
}

Image::Img::Img() : id(0), x(0), y(0)
{

}

Image::Img::Img(const std::vector<u8>& buf) : Img(Image::Load(buf))
{
}

Image::Img::Img(const std::string& path) : Img(Image::Load(path))
{
}

Image::Img::Img(Img&& other) 
{
	*this = std::move(other);
}

Image::Img& Image::Img::operator=(Img&& other)
{
	id = other.id;
	other.id = 0;
	x = other.x;
	y = other.y;
	return *this;
}

Image::Img::~Img()
{
	if (id)
		Image::Free(id);
}

constexpr GLuint Image::Img::ID() const
{
	return id;
}

ImVec2 Image::Img::Size() const
{
	return {x,y};
}

constexpr Image::Img::operator GLuint() const
{
	return id;
}

Image::Img::operator ImTextureID() const
{
	return (ImTextureID)(intptr_t)id;
}

constexpr Image::Img::Img(LoadResult res) : Img(res.id, res.x, res.y)
{
}

constexpr Image::Img::Img(GLuint id, int x, int y) : id(id), x(x), y(y)
{
	if (!id || !x || !y)
		throw std::runtime_error("null texture id or size");
}

#pragma GCC diagnostic ignored "-Wsign-compare"
#pragma GCC diagnostic ignored "-Wmaybe-uninitialized"
#pragma GCC diagnostic ignored "-Wmisleading-indentation"
#pragma GCC diagnostic ignored "-Wunused-but-set-variable"
#define STB_IMAGE_IMPLEMENTATION
#include "stb_image.h"