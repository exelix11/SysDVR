#pragma once

#include <stdint.h>

//8 KB
//TODO: for UDP streaming it should be even lower
#define MaxRTPPacketSize 0x2000
#define RTPHeaderSz 12
#define MaxRTPPayloadSz (MaxRTPPacketSize - RTPHeaderSz)

static uint16_t SequenceNumbers[2];
static const char PT[2] = {96, 97};

static inline uint32_t SwapBytes(uint32_t x)
{
	return ((x & 0x000000ff) << 24) |
		((x & 0x0000ff00) << 8) |
		((x & 0x00ff0000) >> 8) |
		((x & 0xff000000) >> 24);
}

static inline void RTP_PrepareHeader(char* header, uint32_t ts, bool marker, int streamId)
{
	header[0] = 2 << 6;
	header[1] = PT[streamId];
	if (marker) header[1] |= 0x80;

	header[2] = SequenceNumbers[streamId] >> 8;
	header[3] = SequenceNumbers[streamId] & 0xFF;
	SequenceNumbers[streamId]++;

	*((uint32_t*)(header)+1) = SwapBytes(ts);
	*((uint32_t*)(header)+2) = 0;
}