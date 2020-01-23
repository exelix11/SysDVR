#pragma once

#include "RTP.h"
#include "defines.h"

typedef int (*H264SendPacketFn)(const void* header, const void* extHeader, const size_t extLen, const void* data, const size_t len);

#define FU_START 0x80
#define FU_END 0x40
#define FU_HEADER_SZ 2

static inline int PacketizeH264(char* nal, size_t len, uint32_t tsMs, H264SendPacketFn cb)
{
	//Strip nal header
	for (int i = 2; i < len; i++)
		if (nal[i - 2] == 0 && nal[i - 1] == 0 && nal[i] == 1)
		{
			nal += i + 1;
			len -= i + 1;
			break;
		}

	char header[RTPHeaderSz];

	if (len <= MaxRTPPayloadSz)
	{
		//ts is in ms, convert to seconds /1000 and sample at 90khz *90000
		RTP_PrepareHeader(header, tsMs * 90.0, true, STREAM_VIDEO);
		return cb(header, nullptr, 0, nal, len);
	}

	unsigned char FU_A[2] = { 0,0 };
#define fu_indicator FU_A[0]
#define fu_header FU_A[1]

	fu_indicator = (nal[0] & 0xE0) | 28;
	fu_header = nal[0] & 0x1F;

	nal++; len--;

	fu_header |= FU_START;
	while (len > 0)
	{
		size_t dataLen = MaxRTPPayloadSz - FU_HEADER_SZ;

		if (len <= dataLen)
		{
			fu_header |= FU_END;
			dataLen = len;
		}

		RTP_PrepareHeader(header, tsMs * 90.0, true, STREAM_VIDEO);
		if (cb(header, FU_A, 2, nal, dataLen)) return 1;

		nal += dataLen;
		len -= dataLen;
		fu_header &= 0x1F;
	}
	return 0;
#undef fu_indicator
#undef fu_header
}