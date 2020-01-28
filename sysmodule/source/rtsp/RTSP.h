#pragma once
#include <stdint.h>

#if defined(USB_ONLY)
#pragma error This should not be included
#endif

#if defined(__SWITCH__)
#include <stdatomic.h>
#else 
#include <stdbool.h>
typedef bool atomic_bool;
#endif

extern atomic_bool RTSP_ClientStreaming;

//Create and assign RTSPSock before launching the thread
void* RTSP_ServerThread(void*);
void RTSP_StopServer();

//Packetizer callbacks
int RTSP_H264SendPacket(const void* header, const void* extHeader, const size_t extLen, const void* data, const size_t len);
int RTSP_LE16SendPacket(const void* header, const void* data, const size_t len);