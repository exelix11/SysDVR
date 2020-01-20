#pragma once
#include <stdint.h>

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

//Send data over the data channel, RTCP is not used currently
void RTSP_SendBinaryHeader(const size_t totalLen, unsigned int stream);
void RTSP_SendRawData(const void* const data, const size_t len);
void RTSP_SendData(const void* const data, const size_t len, unsigned char stream);