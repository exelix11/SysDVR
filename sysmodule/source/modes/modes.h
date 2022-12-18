#pragma once
#include <switch.h>
#include "defines.h"
#include <stdint.h>
#include <stdbool.h>

#if defined(__SWITCH__)
	#include <stdatomic.h>
#endif

#if defined(USB_ONLY)
static const bool IsThreadRunning = true;
#else
extern atomic_bool IsThreadRunning;
#endif

void LaunchExtraThread(Thread* t, ThreadFunc f, void* arg);
void JoinThread(Thread* t);

typedef struct
{
	void (*InitFn)();
	void (*ExitFn)();
	void (*VThread)(void*);
	void (*AThread)(void*);
	void* Vargs;
	void* Aargs;
} StreamMode;

extern const StreamMode USB_MODE;

#if !defined(USB_ONLY)
extern const StreamMode TCP_MODE;
extern const StreamMode RTSP_MODE;
#endif