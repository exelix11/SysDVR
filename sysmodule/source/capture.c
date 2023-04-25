#include <string.h>

#include "grcd.h"
#include "capture.h"
#include "modes/defines.h"
#include <stdatomic.h>

VideoPacket alignas(0x1000) VPkt;
AudioPacket alignas(0x1000) APkt;

const int DefaultSeqStaticDropThreshold = 5;

static int AudioBatching = MaxABatching;
static int SeqStaticDropThreshold = DefaultSeqStaticDropThreshold;

static Service grcdVideo;
static Service grcdAudio;

static const uint8_t SPS[] = { 0x00, 0x00, 0x00, 0x01, 0x67, 0x64, 0x0C, 0x20, 0xAC, 0x2B, 0x40, 0x28, 0x02, 0xDD, 0x35, 0x01, 0x0D, 0x01, 0xE0, 0x80 };
static const uint8_t PPS[] = { 0x00, 0x00, 0x00, 0x01, 0x68, 0xEE, 0x3C, 0xB0 };

static bool forceSPSPPS = false;

static atomic_bool capturing = false;

bool CaptureReadAudio()
{
	Result rc = grcdServiceTransfer(
		&grcdAudio, GrcStream_Audio,
		APkt.Data, AbufSz,
		NULL,
		&APkt.Header.DataSize,
		&APkt.Header.Timestamp);

	if (R_FAILED(rc))
		return false;

	for (int i = 0; i < AudioBatching; i++)
	{
		u32 tmpSize = 0;

		rc = grcdServiceTransfer(
			&grcdAudio, GrcStream_Audio,
			APkt.Data + AbufSz + (AbufSz * i), AbufSz,
			NULL,
			&tmpSize,
			NULL);

		APkt.Header.DataSize += tmpSize;
	}

	return R_SUCCEEDED(rc);
}

#define CRC32X(crc, value) __asm__("crc32x %w[c], %w[c], %x[v]":[c]"+r"(crc):[v]"r"(value))
#define CRC32W(crc, value) __asm__("crc32w %w[c], %w[c], %w[v]":[c]"+r"(crc):[v]"r"(value))
#define CRC32H(crc, value) __asm__("crc32h %w[c], %w[c], %w[v]":[c]"+r"(crc):[v]"r"(value))
#define CRC32B(crc, value) __asm__("crc32b %w[c], %w[c], %w[v]":[c]"+r"(crc):[v]"r"(value))

#define DEFINE_UNALIGNED_ACCESSOR(type) static type get_unaligned_##type(const u8 *p) { type v; memcpy(&v, p, sizeof(v)); return v; }

DEFINE_UNALIGNED_ACCESSOR(u64);
DEFINE_UNALIGNED_ACCESSOR(u32);
DEFINE_UNALIGNED_ACCESSOR(u16);

static u32 crc32_arm64_hw(u32 crc, const u8* p, unsigned int len)
{
	s64 length = len;
	while ((length -= sizeof(u64)) >= 0) {
		CRC32X(crc, get_unaligned_u64(p));
		p += sizeof(u64);
	}
	/* The following is more efficient than the straight loop */
	if (length & sizeof(u32)) {
		CRC32W(crc, get_unaligned_u32(p));
		p += sizeof(u32);
	}
	if (length & sizeof(u16)) {
		CRC32H(crc, get_unaligned_u16(p));
		p += sizeof(u16);
	}
	if (length & sizeof(u8))
		CRC32B(crc, *p);
	return crc;
}

bool CheckVideoPacket(const u8* data, size_t len)
{
	if (SeqStaticDropThreshold == 0)
		return false;

	static u32 droppedInAReow = 0;
	static u32 LastPackets[15];
	static u32 LastPacketsIdx = 0;

	u32 hash = crc32_arm64_hw(0, data, len);
	bool found = false;

	for (int i = 0; i < sizeof(LastPackets) / sizeof(LastPackets[0]); i++)
	{
		if (LastPackets[i] == hash)
		{
			found = true;
			break;
		}
	}

	LastPackets[LastPacketsIdx++] = hash;
	LastPacketsIdx %= sizeof(LastPackets) / sizeof(LastPackets[0]);

	// We need to send these keyframes once in a while otherwise we get something similar to https://github.com/exelix11/SysDVR/issues/91
	if (found && ++droppedInAReow > SeqStaticDropThreshold)
	{
		droppedInAReow = 0;
		LOG("Letting duplicate packet through.\n");
		return false;
	}

	return found;
}

bool CaptureReadVideo()
{
again:
	Result res = grcdServiceTransfer(
		&grcdVideo, GrcStream_Video,
		VPkt.Data, VbufSz,
		NULL,
		&VPkt.Header.DataSize,
		&VPkt.Header.Timestamp);

	bool result = R_SUCCEEDED(res) && VPkt.Header.DataSize > 4;

#ifndef RELEASE
	// Sometimes the buffer is too small for IDR frames causing this https://github.com/exelix11/SysDVR/issues/91 
	// These big NALs are not common and even if they're missed they only cause graphical glitches, it's better not to fatal in release builds
	// Error code should be 2212-0006
	if (R_FAILED(res))
		fatalThrow(res);
#endif

	if (!result) {
		LOG("Video capture failed: %x\n", res);
		return false;
	}

	// Static images are particularly bad for sysdvr cause they cause the video encoder
	// to emit really big keyframes, all to produce a static image. So here's a trick:
	// We hash these big blocks and keep the last 10 or os, if they keep repeating we know
	// this is a static images so we kan just not send it with no consequences to free
	// up bandwidth for audio and reduce delay once the image changes
	if (VPkt.Header.DataSize > 0xF000 && CheckVideoPacket(VPkt.Data, VPkt.Header.DataSize)) {
		LOG("Dropping duplicate video packet\n");
		goto again;
	}

	/*
		GRC only emits SPS and PPS once when a game is started,
		this is not good as without those it's not possible to play the stream If there's space add SPS and PPS to IDR frames every once in a while
	*/
	static int IDRCount = 0;
	const bool isIDRFrame = (VPkt.Data[4] & 0x1F) == 5;

	// if this is an IDR frame and we haven't added SPS/PPS in the last 5 or forceSPSPPS is set
	bool EmitMeta = forceSPSPPS || (isIDRFrame && ++IDRCount >= 5);

	// Only if there's enough space
	if (EmitMeta && (VbufSz - VPkt.Header.DataSize) >= (sizeof(PPS) + sizeof(SPS)))
	{
		IDRCount = 0;
		forceSPSPPS = false;
		memmove(VPkt.Data + sizeof(PPS) + sizeof(SPS), VPkt.Data, VPkt.Header.DataSize);
		memcpy(VPkt.Data, SPS, sizeof(SPS));
		memcpy(VPkt.Data + sizeof(SPS), PPS, sizeof(PPS));
		VPkt.Header.DataSize += sizeof(SPS) + sizeof(PPS);
	}

	return result;
}

// Configurable options
void CaptureSetAudioBatching(int batch)
{
	if (batch < 0)
		batch = 0;

	if (batch > MaxABatching)
		batch = MaxABatching;

	AudioBatching = batch;
}

int CaptureGetAudioBatching()
{
	return AudioBatching;
}

void CaptureResetStaticDropThreshold()
{
	SeqStaticDropThreshold = DefaultSeqStaticDropThreshold;
}

void CaptureSetStaticDropThreshold(int maxConsecutive) 
{
	SeqStaticDropThreshold = maxConsecutive;
}

int CaptureGetStaticDropThreshold()
{
	return SeqStaticDropThreshold;
}

Result CaptureInitialize()
{
	capturing = true;

	R_RET_ON_FAIL(grcdServiceOpen(&grcdVideo));
	R_RET_ON_FAIL(grcdServiceOpen(&grcdAudio));
	return grcdServiceBegin(&grcdVideo);
}

void CaptureFinalize()
{
	grcdServiceClose(&grcdVideo);
	grcdServiceClose(&grcdAudio);
}