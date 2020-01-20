#pragma once

#include "RTP.h"
#include "defines.h"

typedef void (*LE16SendPacketFn)(void* header, void* data, size_t len);

#define SampleRate 48000.0
#define SamplesInRtpPacket (MaxRTPPayloadSz / 4.0)
#define AudiolenRtpPackeMs (SamplesInRtpPacket / SampleRate * 1000)

static inline void PacketizeLE16(char* data, size_t len, uint32_t ts, LE16SendPacketFn cb)
{
	char header[RTPHeaderSz];
	double incrementalTs = 0;

	while (len > 0)
	{
		size_t dataLen = len > MaxRTPPayloadSz ? MaxRTPPayloadSz : len;

		uint8_t* samples = (uint8_t*)data;
		for (int i = 0; i < dataLen; i += 2)
		{
			uint8_t tmp = data[i];

			samples[i] = samples[i + 1];
			samples[i + 1] = tmp;
		}

		//ts for audio seems to be in usec, while it's msec for video (?)
		RTP_PrepareHeader(header, (ts / 1000.0 + incrementalTs) * 48 / 1000, false, STREAM_AUDIO);
		cb(header, data, dataLen);

		data += dataLen;
		len -= dataLen;
		incrementalTs += AudiolenRtpPackeMs;
	}
}
