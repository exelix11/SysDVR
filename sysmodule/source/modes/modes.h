#pragma once
#include "modeDefines.h"
#include <stdint.h>
#include <stdbool.h>

#if defined(__SWITCH__)
	#include <stdatomic.h>
#endif

#if defined(USB_ONLY)
static const bool IsThreadRunning = true;
#else
/*
	Accessing this is rather slow, avoid using it in the main flow of execution.
	When stopping the main thread will close the sockets or dispose the usb interfaces, causing the others to fail
	Only in that case this variable should be checked
*/
extern atomic_bool IsThreadRunning;
#endif

#define VbufSz 0x32000
#define AbufSz 0x1000
#define AudioBatchSz 10

extern uint8_t Vbuf[]; //VbufSz
extern uint8_t Abuf[]; //AbufSz * AudioBatchSz
extern uint32_t VOutSz;
extern uint32_t AOutSz;

//Note: timestamps are in usecs
extern uint64_t VTimestamp;
extern uint64_t ATimestamp;

void ReadAudioStream();
void ReadVideoStream();

typedef struct
{
	void (*InitFn)();
	void (*ExitFn)();
	void* (*MainThread)(void*);
} StreamMode;

extern StreamMode USB_MODE;
#if !defined(USB_ONLY)
extern StreamMode TCP_MODE;
extern StreamMode RTSP_MODE;

static const uint8_t SPS[] = { 0x00, 0x00, 0x00, 0x01, 0x67, 0x64, 0x0C, 0x20, 0xAC, 0x2B, 0x40, 0x28, 0x02, 0xDD, 0x35, 0x01, 0x0D, 0x01, 0xE0, 0x80 };
static const uint8_t PPS[] = { 0x00, 0x00, 0x00, 0x01, 0x68, 0xEE, 0x3C, 0xB0 };
#endif