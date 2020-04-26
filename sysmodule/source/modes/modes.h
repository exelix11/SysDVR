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
/*
	Accessing this is rather slow, avoid using it in the main flow of execution.
	When stopping the main thread will close the sockets or dispose the usb interfaces, causing the others to fail
	Only in that case this variable should be checked
*/
extern atomic_bool IsThreadRunning;
#endif

#define TerminateOrContinue {if (IsThreadRunning) continue; else break;}

#define VbufSz 0x32000
#define AbufSz 0x1000
#define AMaxBatch 4

typedef struct {
	u32 Magic;
	u32 DataSize;
	u64 Timestamp; //Note: timestamps are in usecs
} PacketHeader;

_Static_assert(sizeof(PacketHeader) == 16); //Ensure no padding, PACKED triggers a warning

typedef struct {
	PacketHeader Header;
	u8 Data[VbufSz];
} VideoPacket;

_Static_assert(sizeof(VideoPacket) == sizeof(PacketHeader) + VbufSz);

typedef struct {
	PacketHeader Header;
	u8 Data[AbufSz * AMaxBatch];
} AudioPacket;

_Static_assert(sizeof(AudioPacket) == sizeof(PacketHeader) + AbufSz * AMaxBatch);

extern VideoPacket VPkt;
extern AudioPacket APkt;

void SetAudioBatching(int count);
bool ReadAudioStream();
bool ReadVideoStream();

void LaunchThread(Thread* t, ThreadFunc f);
void JoinThread(Thread* t);

typedef struct
{
	void (*InitFn)();
	void (*ExitFn)();
	void (*VThread)(void*);
	void (*AThread)(void*);
} StreamMode;

extern StreamMode USB_MODE;

#if !defined(USB_ONLY)
static const uint8_t SPS[] = { 0x00, 0x00, 0x00, 0x01, 0x67, 0x64, 0x0C, 0x20, 0xAC, 0x2B, 0x40, 0x28, 0x02, 0xDD, 0x35, 0x01, 0x0D, 0x01, 0xE0, 0x80 };
static const uint8_t PPS[] = { 0x00, 0x00, 0x00, 0x01, 0x68, 0xEE, 0x3C, 0xB0 };

extern StreamMode TCP_MODE;
extern StreamMode RTSP_MODE;
#endif