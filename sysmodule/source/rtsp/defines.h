#pragma once

#define STREAM_VIDEO 0
#define STREAM_AUDIO 1
#define nullptr ((void*)NULL)

//1KB
#define MaxRTPPacketSize_UDP 0x400
//16KB
#define MaxRTPPacketSize_TCP 0x4000
#define RTPHeaderSz 12

#if defined(USB_ONLY)
#pragma error This should not be included
#endif