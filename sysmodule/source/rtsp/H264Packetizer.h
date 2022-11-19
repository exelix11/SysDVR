#pragma once

#include "RTP.h"

typedef int (*H264SendPacketFn)(const void* header, const size_t headerLen, const void* data, const size_t len);

#define FU_START 0x80
#define FU_END 0x40
#define FU_HEADER_SZ 2

static inline int PacketizeH264Single(const char* nal, size_t len, uint32_t tsMs, H264SendPacketFn cb)
{
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

static inline s32 FindNextNalHeaderOffset(const char* data, size_t len)
{
	for (int i = 2; i < len; i++)
		if (data[i - 2] == 0 && data[i - 1] == 0 && data[i] == 1)
			return i + 1;

	return -1;
}

static inline bool PacketizeH264(const char* data, size_t len, uint32_t tsMs, H264SendPacketFn cb)
{
	s32 CurrentOffset = FindNextNalHeaderOffset(data, len);
	while (CurrentOffset >= 0)
	{	
		s32 nextOffset = FindNextNalHeaderOffset(data + CurrentOffset, len - CurrentOffset);
		if (nextOffset < 0)
			return PacketizeH264Single(data + CurrentOffset, len - CurrentOffset, tsMs, cb);

		nextOffset += CurrentOffset;
		s32 startNext = nextOffset - 3;
		s32 curSize = startNext - CurrentOffset;

		if (PacketizeH264Single(data + CurrentOffset, curSize, tsMs, cb))
			return 1;

		CurrentOffset = nextOffset;
	}
	return 0;
}