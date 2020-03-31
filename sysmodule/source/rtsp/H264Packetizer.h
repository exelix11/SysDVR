#pragma once

#include "RTP.h"

typedef int (*H264SendPacketFn)(const void* header, const size_t headerLen, const void* data, const size_t len);

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

	char header[RTPHeaderSz + 2]; // 2 extra bytes for the FU-A header, not really part of the RTP header but saves us a syscall on tcp and a memcopy on udp

	if (len <= MaxRTPPayload)
	{
		//ts is in ms, convert to seconds /1000 and sample at 90khz *90000
		RTP_PrepareHeader(header, tsMs * 90.0, (*nal & 0x1F) <= 5, STREAM_VIDEO);
		return cb(header, RTPHeaderSz, nal, len); //FU-A header not used
	}

#define fu_indicator header[RTPHeaderSz + 0]
#define fu_header header[RTPHeaderSz + 1]

	fu_indicator = (nal[0] & 0xE0) | 28;
	fu_header = nal[0] & 0x1F;

	nal++; len--;

	fu_header |= FU_START;
	while (len > 0)
	{
		size_t dataLen = MaxRTPPayload - FU_HEADER_SZ;

		if (len <= dataLen)
		{
			fu_header |= FU_END;
			dataLen = len;
		}

		RTP_PrepareHeader(header, tsMs * 90.0, fu_header & FU_END, STREAM_VIDEO);
		if (cb(header, RTPHeaderSz + 2, nal, dataLen)) return 1;

		nal += dataLen;
		len -= dataLen;
		fu_header &= 0x1F;
	}
	return 0;
#undef fu_indicator
#undef fu_header
}