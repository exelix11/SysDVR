#include "utils.h"
#include <switch.h>

bool CreateDummyFile(const char* fname)
{
	FILE* f = fopen(fname, "w");
	if (f)
	{
		fwrite(".", 1, 1, f);
		fclose(f);
		return true;
	}
	return false;
}

bool FileExists(const char* fname)
{
	FILE* f = fopen(fname, "rb");
	if (f)
	{
		fclose(f);
		return true;
	}
	return false;
}