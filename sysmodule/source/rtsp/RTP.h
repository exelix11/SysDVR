#pragma once
#include <stdint.h>
#include "../modes/modes.h"

#if defined(USB_ONLY)
#pragma error "This should not be included"
#endif

#define STREAM_VIDEO 0
#define STREAM_AUDIO 1

#define RTPHeaderSz 12
#define MaxRTPPayload (MaxRTPPacket - RTPHeaderSz)

extern uint16_t SequenceNumbers[2];
static const char PT[2] = {96, 97};

static inline void RTP_InitializeSequenceNumbers() 
{
	SequenceNumbers[0] = 1;
	SequenceNumbers[1] = 1;
}

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