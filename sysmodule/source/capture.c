#include <string.h>

#include "core.h"
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

// User configuration
static int AudioBatching = MaxABatching;
static bool InjectSpsPps = true;
static bool HashNals = false;

static void PacketMakeHash(PacketHeader* packet, u32 hash)
{
	packet->Meta.Struct.Content = PacketContent_Hash;
	packet->DataSize = sizeof(hash);
	memcpy(packet + sizeof(PacketHeader), &hash, sizeof(hash));
}

static void PacketMakeData(PacketHeader* packet)
{
	packet->Meta.Struct.Content = PacketContent_Data;
	packet->Meta.Struct.Channel = PacketType_Audio;
}

void CaptureAudioConnected()
{
	APkt.Header.Magic = STREAM_PACKET_HEADER;
}

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

static u32 NalHashes[20];
static u32 NalHashIdx = 0;

bool CheckVideoPacket(const u8* data, size_t len, u32* outHash)
{
	if (HashNals)
		return false;

	u32 hash = crc32_arm64_hw(0, data, len);
	bool found = false;

	for (int i = 0; i < sizeof(NalHashes) / sizeof(NalHashes[0]); i++)
	{
		if (NalHashes[i] == hash)
		{
			*outHash = hash;
			found = true;
			break;
		}
	}

	NalHashes[NalHashIdx++] = hash;
	NalHashIdx %= sizeof(NalHashes) / sizeof(NalHashes[0]);

	return found;
}

void CaptureVideoConnected()
{
	memset(NalHashes, 0, sizeof(NalHashes));
	NalHashIdx = 0;
	VPkt.Header.Magic = STREAM_PACKET_HEADER;
	VPkt.Header.Meta.Struct.Channel = PacketType_Video;
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

	// GRC only produces a single nal per packet so this is always correct
	const bool isIDRFrame = (VPkt.Data[4] & 0x1F) == 5;

	// Static images are particularly bad for sysdvr cause they cause the video encoder
	// to emit really big keyframes, all to produce a static image. So here's a trick:
	// We hash these big blocks and keep the last 10 or os, if they keep repeating we know
	// this is a static images so we kan just not send it with no consequences to free
	// up bandwidth for audio and reduce delay once the image changes
	u32 hash;
	if (isIDRFrame && CheckVideoPacket(VPkt.Data, VPkt.Header.DataSize, &hash)) {
		LOG("Sending packet hash instead\n");
		PacketMakeHash(&VPkt.Header, hash);
		return true;
	}

	PacketMakeData(&VPkt.Header);

	/*
		GRC only emits SPS and PPS once when a game is started,
		this is not good as without those it's not possible to play the stream If there's space add SPS and PPS to IDR frames every once in a while
	*/
	if (InjectSpsPps) {
		static int IDRCount = 0;

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
			VPkt.Header.Meta.Struct.Content = PacketContent_MultiNAL;
		}
	}

	return result;
}

// Configurable options
int CaptureSetAudioBatching(int batch)
{
	if (batch < 0)
		batch = 0;

	if (batch > MaxABatching)
		batch = MaxABatching;

	LOG("CaptureSetAudioBatching(%d)\n", batch);
	AudioBatching = batch;
	return AudioBatching;
}

void CaptureSetPPSSPSInject(bool value)
{
	LOG("CaptureSetPPSSPSInject(%d)\n", value);
	InjectSpsPps = value;
}

void CaptureSetNalHashing(bool value)
{
	LOG("CaptureSetNalHashing(%d)\n", value);
	HashNals = value;
}

void CaptureConfigResetDefault()
{
	CaptureSetNalHashing(true);
	CaptureSetPPSSPSInject(true);
	CaptureSetAudioBatching(MaxABatching);
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
