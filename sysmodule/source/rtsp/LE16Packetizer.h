#pragma once

#include "RTP.h"
#include "defines.h"

typedef int (*LE16SendPacketFn)(const void* header, const void* data, const size_t len);

#define SampleRate 48000.0
#define SamplesInRtpPacket(payloadSz) (payloadSz / 4.0)
#define AudiolenRtpPackeMs(payloadSz) (SamplesInRtpPacket(payloadSz) / SampleRate * 1000.0)

static inline int PacketizeLE16(char* data, size_t len, uint32_t tsMs, LE16SendPacketFn cb)
{
	char header[RTPHeaderSz];
	double incrementalTs = 0;

	const double increment = AudiolenRtpPackeMs(MaxRTPPayload);

	while (len > 0)
	{
		size_t dataLen = len > MaxRTPPayload ? MaxRTPPayload : len;

		uint8_t* samples = (uint8_t*)data;
		for (int i = 0; i < dataLen; i += 2)
		{
			uint8_t tmp = data[i];

			samples[i] = samples[i + 1];
			samples[i + 1] = tmp;
		}

		//ts is in ms, convert to seconds /1000 and sample at 48khz *48000
		RTP_PrepareHeader(header, (uint32_t)(((double)tsMs + incrementalTs) * 48.0), false, STREAM_AUDIO);
		if (cb(header, data, dataLen)) 
			return 1;

		data += dataLen;
		len -= dataLen;
		incrementalTs += increment;
	}
	return 0;
}
