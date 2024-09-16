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
static int AudioBatching = DefaultABatching;
static bool InjectSpsPps = true;
static bool HashNals = false;
static bool HashOnlyKeyframes = true;

static void PacketMakeReplay(VideoPacket* packet)
{
	packet->Header.MetaData = (packet->Header.MetaData & ~PacketMeta_Content_Mask) | PacketMeta_Content_Replay;
	packet->Header.DataSize = 0;
}

static void PacketMakeData(PacketHeader* packet)
{
	packet->MetaData = (packet->MetaData & ~PacketMeta_Content_Mask) | PacketMeta_Content_Data;
}

static void PacketMakeError(PacketHeader* packet, void* packedBody, u32 code, u64 context1, u64 context2)
{
	ErrorPacket error = {
		.ErrorType = ERROR_TYPE_RESULT,
		.ErrorCode = code,
		.Context1 = context1,
		.Context2 = context2,
	};

	packet->MetaData = (packet->MetaData & ~PacketMeta_Content_Mask) | PacketMeta_Content_Error;
	packet->DataSize = sizeof(error);
	
	memcpy(packedBody, &error, sizeof(error));
}

void CaptureAudioConnected()
{
	APkt.Header.Magic = STREAM_PACKET_HEADER;
	APkt.Header.MetaData = PacketMeta_Type_Audio | PacketMeta_Content_Data;
	APkt.Header.ReplaySlot = 0xFF;
}

bool CaptureReadAudio()
{
	u32 dataSize; u64 timestamp;

	Result rc = grcdServiceTransfer(
		&grcdAudio, GrcStream_Audio,
		APkt.Data, AbufSz,
		NULL,
		&dataSize,
		&timestamp);

	APkt.Header.DataSize = dataSize;
	APkt.Header.Timestamp = timestamp;

	if (!R_SUCCEEDED(rc))
	{
		LOG("Audio capture failed: %x size: %x\n", rc, dataSize);
		PacketMakeError(&APkt.Header, APkt.Data, rc, dataSize, 0);
		return false;
	}

	for (int i = 0; i < AudioBatching; i++)
	{
		u32 tmpSize = 0;

		rc = grcdServiceTransfer(
			&grcdAudio, GrcStream_Audio,
			APkt.Data + AbufSz + (AbufSz * i), AbufSz,
			NULL,
			&tmpSize,
			NULL);

		if (!R_SUCCEEDED(rc))
		{
			LOG("Audio capture failed: %x size: %x (loop %d)\n", rc, tmpSize, i);
			PacketMakeError(&APkt.Header, APkt.Data, rc, tmpSize, i + 1);
			return false;
		}

		APkt.Header.DataSize += tmpSize;
	}

	return true;
}

#define CRC32X(crc, value) __asm__("crc32x %w[c], %w[c], %x[v]":[c]"+r"(crc):[v]"r"(value))
#define CRC32W(crc, value) __asm__("crc32w %w[c], %w[c], %w[v]":[c]"+r"(crc):[v]"r"(value))
#define CRC32H(crc, value) __asm__("crc32h %w[c], %w[c], %w[v]":[c]"+r"(crc):[v]"r"(value))
#define CRC32B(crc, value) __asm__("crc32b %w[c], %w[c], %w[v]":[c]"+r"(crc):[v]"r"(value))

#define DEFINE_UNALIGNED_ACCESSOR(type) static type get_unaligned_##type(const u8 *p) { type v; memcpy(&v, p, sizeof(v)); return v; }

DEFINE_UNALIGNED_ACCESSOR(u64);
DEFINE_UNALIGNED_ACCESSOR(u32);
DEFINE_UNALIGNED_ACCESSOR(u16);

u32 crc32_arm64_hw(const u8* p, unsigned int len)
{
	u32 crc = 0xffffffff;

	s64 length = len;
	while (length >= sizeof(u64)) {
		CRC32X(crc, get_unaligned_u64(p));
		p += sizeof(u64);
		length -= sizeof(u64);
	}
	
	if (length >= sizeof(u32)) {
		CRC32W(crc, get_unaligned_u32(p));
		p += sizeof(u32);
		length -= sizeof(u32);
	}

	if (length >= sizeof(u16)) {
		CRC32H(crc, get_unaligned_u16(p));
		p += sizeof(u16);
		length -= sizeof(u16);
	}

	if (length)
		CRC32B(crc, *p);
	
	return ~crc;
}

#define HASH_BITS 6
static u32 NalHashes[1 << HASH_BITS];

bool HashVideoPacket(const u8* data, size_t len, u8* outSlot)
{
	u32 hash = crc32_arm64_hw(data, len);

	// We use the lower HASH_BITS bits of the hash as the index
	int index = hash & ((1 << HASH_BITS) - 1);
	*outSlot = (u8)index;

	// If the hash is already in the table we do not need to send the packet again
	if (NalHashes[index] == hash)
		return true;

	// Otherwise we update the hash table and send the packet
	NalHashes[index] = hash;

	return false;
}

void CaptureVideoConnected()
{
	memset(NalHashes, 0, sizeof(NalHashes));
	VPkt.Header.Magic = STREAM_PACKET_HEADER;
	VPkt.Header.MetaData = PacketMeta_Type_Video;
	VPkt.Header.ReplaySlot = 0xFF;
	forceSPSPPS = true;
}

bool CaptureReadVideo()
{
	u32 dataSize; u64 timestamp;

	Result res = grcdServiceTransfer(
		&grcdVideo, GrcStream_Video,
		VPkt.Data, VbufSz,
		NULL,
		&dataSize,
		&timestamp);

	VPkt.Header.DataSize = dataSize;
	VPkt.Header.Timestamp = timestamp;
	VPkt.Header.ReplaySlot = 0xFF;

	bool result = R_SUCCEEDED(res) && VPkt.Header.DataSize > 4;

	// Sometimes the buffer is too small for IDR frames causing this https://github.com/exelix11/SysDVR/issues/91 
	// These big NALs are not common and even if they're missed they only cause graphical glitches
	// Error code should be 2212-0006
	if (!result) 
	{
		LOG("Video capture failed: %x size: %x\n", res, dataSize);
		// In these cases, set the meta error flag and the payload is just the size 
		PacketMakeError(&VPkt.Header, VPkt.Data, res, dataSize, 0);
	}
	else
	{
		// GRC only produces a single nal per packet so this is always correct
		const bool isIDRFrame = (VPkt.Data[4] & 0x1F) == 5;

		// Static images are particularly bad for sysdvr cause they cause the video encoder
		// to emit really big keyframes, all to produce a static image. So here's a trick:
		// We hash these big blocks and keep the last 10 or os, if they keep repeating we know
		// this is a static images so we can just not send it with no consequences to free
		// up bandwidth for audio and reduce delay once the image changes
		const bool UseHash = HashNals && (isIDRFrame || !HashOnlyKeyframes);

		if (UseHash && HashVideoPacket(VPkt.Data, VPkt.Header.DataSize, &VPkt.Header.ReplaySlot)) {
			LOG("Sending packet hash instead\n");
			PacketMakeReplay(&VPkt);
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
				VPkt.Header.MetaData |= PacketMeta_Content_MultiNal;
			}
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

void CaptureSetNalHashing(bool enabled, bool onlyKeyframes)
{
	LOG("CaptureSetNalHashing(%d, %d)\n", enabled, onlyKeyframes);
	HashNals = enabled;
	HashOnlyKeyframes = onlyKeyframes;
}

void CaptureConfigResetDefault()
{
	CaptureSetNalHashing(true, true);
	CaptureSetPPSSPSInject(true);
	CaptureSetAudioBatching(DefaultABatching);
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
