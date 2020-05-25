#pragma once
#include <stdint.h>

#include "RTP.h"

#if defined(__SWITCH__)
#include <stdatomic.h>
#else 
#include <stdbool.h>
typedef bool atomic_bool;
#endif

extern atomic_bool RTSP_ClientStreaming;

void RTSP_ServerThread(void*);
void RTSP_StopServer();

//Packetizer callbacks
int RTSP_H264SendPacket(const void* header, const size_t headerLen, const void* data, const size_t len);
int RTSP_LE16SendPacket(const void* header, const void* data, const size_t len);