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

void LaunchThread(Thread* t, ThreadFunc f, void* arg, void* stackLocation, u32 stackSize, u32 prio);
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

#define MaxRTPPacket 8 * 1024
#define RTSPBinHeaderSize 4

// The various streaming modes require specific memory buffers
// since we want to reduce dynamic allocations and wasted memory
// this data structure is used to store all the buffers as an union
// so that we can use the same memory for different modes
// During mode switching this area is zeroed out
typedef union {
	struct {
		// Since this is zeroed by default flags should use false as default state
		bool IsInUse;
		u8 alignas(0x1000) EndpointIn[0x1000];
		u8 alignas(0x1000) EndpointOut[0x1000];
	} UsbMode;
// Since this is the biggest contributor ifdef it out when not needed
#ifndef USB_ONLY
	struct {
		char VideoSendBuffer[MaxRTPPacket + RTSPBinHeaderSize];
		char AudioSendBuffer[MaxRTPPacket + RTSPBinHeaderSize];
		u8 alignas(0x1000) ServerThreadStackArea[0x2000];
	} RTSPMode;
#endif
} StaticBuffers;

extern StaticBuffers Buffers;

#if !defined(USB_ONLY)
extern const StreamMode TCP_MODE;
extern const StreamMode RTSP_MODE;
#endif