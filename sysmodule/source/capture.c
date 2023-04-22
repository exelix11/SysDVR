#include <string.h>

#include "grcd.h"
#include "capture.h"
#include "modes/defines.h"
#include <stdatomic.h>

VideoPacket alignas(0x1000) VPkt;
AudioPacket alignas(0x1000) APkt;

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

	for (int i = 1; i < ABatching; i++)
	{
		u32 tmpSize = 0;

		rc = grcdServiceTransfer(
			&grcdAudio, GrcStream_Audio,
			APkt.Data + (AbufSz * i), AbufSz,
			NULL,
			&tmpSize,
			NULL);

		APkt.Header.DataSize += tmpSize;
	}

	return R_SUCCEEDED(rc);
}

bool CaptureReadVideo()
{
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