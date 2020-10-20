#pragma once
#include <string.h>
#include <stdio.h>
#include <unistd.h>
#include <switch.h>

bool CreateDummyFile(const char* fname);
bool FileExists(const char* fname);