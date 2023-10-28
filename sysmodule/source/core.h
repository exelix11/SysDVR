#pragma once
#include <switch.h>
#include "modes/defines.h"

// Debug UDP logging with netcat -ul 9999
#define LOGGING_ENABLED (UDP_LOGGING || FILE_LOGGING || CUSTOM_LOGGING)

#if (UDP_LOGGING + FILE_LOGGING + CUSTOM_LOGGING) > 1
	#pragma error "UDP and file logging are mutually exclusive"
#endif

#define NEEDS_FS (!defined(USB_ONLY) || FILE_LOGGING)
#define NEEDS_SOCKETS (!defined(USB_ONLY) || UDP_LOGGING)

#if LOGGING_ENABLED
	#if FILE_LOGGING
		#include <stdio.h>
		#define LogFunctionImpl(...) do { printf(__VA_ARGS__); fflush(stdout); } while (0)
	#else
		void LogFunctionImpl(const char* fmt, ...);
	#endif

	#define LOG(...) do { LogFunctionImpl(__VA_ARGS__); } while (0)

	#ifdef VERBOSE_LOGGING
		void LogVerboseFunctionImpl(const char* fmt, ...);
		#define LOG_V(...) do { LogVerboseFunctionImpl(__VA_ARGS__); } while (0)
	#else
		#define LOG_V(...) do { } while (0)
	#endif

	// This is also added to thread stacks so must be 0x1000-aligned	
	#define LOGGING_STACK_BOOST 0x1000
#else
#define LOG(...) do { } while (0)
#define LOG_V(...) do { } while (0)
#define LOGGING_STACK_BOOST 0
#endif

#define R_RET_ON_FAIL(x) do { Result rc = x; if (R_FAILED(rc)) return rc; } while (0)
#define R_THROW(x) do { Result r = x; if (R_FAILED(r)) { fatalThrow(r); }  } while(0)

extern char SysDVRBeacon[];
extern int SysDVRBeaconLen;

Result CoreInit();

void LaunchThread(Thread* t, ThreadFunc f, void* arg, void* stackLocation, u32 stackSize, u32 prio);

void JoinThread(Thread* t);

#ifndef USB_ONLY
void SetModeID(u32 mode);

u32 GetCurrentMode();
#endif