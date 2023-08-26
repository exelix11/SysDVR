// SDL.h provides the SDL_Main() symbol and must be kept
#include "SDL.h"
#include <android/log.h>
  
struct NativeInitBlock
{
	void* Version;
	void* PrintFunction;
};
  
extern void sysdvr_entrypoint(struct NativeInitBlock* init);

void LOG(const char* string) 
{
	__android_log_print(ANDROID_LOG_ERROR, "SysDVRLogger", "%s", string);	
}

struct NativeInitBlock g_native = 
{
	.Version = (void*)1,
	.PrintFunction = (void*)LOG
};

int main(int argc, char *argv[])
{
  LOG("sdl main called()");
  sysdvr_entrypoint(&g_native);
  LOG("sysdvr_entrypoint returned");
  return 0;
}
