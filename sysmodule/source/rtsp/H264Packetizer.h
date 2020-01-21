#pragma once

#include "RTP.h"
#include "defines.h"

typedef int (*H264SendPacketFn)(const void* header, const void* extHeader, const size_t extLen, const void* data, const size_t len);

#define FU_START 0x80
#define FU_END 0x40
#define FU_HEADER_SZ 2

static inline int PacketizeH264(char* nal, size_t len, uint32_t ts, H264SendPacketFn cb)
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
		RTP_PrepareHeader(header, ts * 90.0 / 1000, (nal[0] & 0x1f) <= 5, STREAM_VIDEO);
		cb(header, nullptr, 0, nal, len);
		return 1;
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

		RTP_PrepareHeader(header, ts * 90.0 / 1000, fu_header & FU_END, STREAM_VIDEO);
		if (cb(header, FU_A, 2, nal, dataLen)) return 1;

		nal += dataLen;
		len -= dataLen;
		fu_header &= 0x1F;
	}
	return 0;
#undef fu_indicator
#undef fu_header
}