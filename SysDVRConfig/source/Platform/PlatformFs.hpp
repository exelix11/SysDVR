#pragma once

#ifndef __SWITCH__
#define ASSET(_str) "./romfs/" _str
#define SDMC "F:"
typedef unsigned char u8;
#else
#define ASSET(_str) "romfs:/" _str
#define SDMC ""
#include <switch.h>
#endif

#if WIN32
#include <io.h>
#include <direct.h>
#include "Windows\dirent.h"
#define mkdir(x,y) _mkdir(x)
#define rmdir(x) _rmdir(x)
#define unlink(x) _unlink(x)
#undef CreateDirectory
#undef max
#else
#include <unistd.h>
#include <dirent.h>
#endif