#if !defined(USB_ONLY)
#include "modes.h"
#include "../capture.h"
#include "../rtsp/RTSP.h"
#include "../rtsp/H264Packetizer.h"
#include "../rtsp/LE16Packetizer.h"
#include "../net/sockets.h"

static Thread RTSPThread;

static void RTSP_StreamVideo(void* _)
{
	if (!IsThreadRunning)
		fatalThrow(ERR_RTSP_VIDEO);

	while (true)
	{
		u64 firstTs = 0;

		while (!RTSP_ClientStreaming && IsThreadRunning)
			svcSleepThread(1E+8); // 1/10 of second

		if (!IsThreadRunning) break;

		LOG("RTSP VIDEO STREAMING\n");

		while (true)
		{
			if (!CaptureReadVideo() || !IsThreadRunning)
				break;

			if (firstTs == 0)
				firstTs = VPkt.Header.Timestamp;

			LOG_V("RTSP VIDEO TS %lu BYTES %lu\n", VPkt.Header.Timestamp, VPkt.Header.DataSize + sizeof(PacketHeader));
			bool success = !PacketizeH264((char*)VPkt.Data, VPkt.Header.DataSize, (VPkt.Header.Timestamp - firstTs) / 1000, RTSP_H264SendPacket);

			if (!success)
				break;
		}
	}
}

static void RTSP_StreamAudio(void* _)
{
	if (!IsThreadRunning)
		fatalThrow(ERR_RTSP_AUDIO);

	while (true)
	{
		u64 firstTs = 0;

		while (!RTSP_ClientStreaming && IsThreadRunning)
			svcSleepThread(1E+8); // 1/10 of second

		if (!IsThreadRunning)
			break;

		LOG("RTSP AUDIO STREAMING\n");

		while (IsThreadRunning)
		{
			if (!CaptureReadAudio() || !IsThreadRunning)
				break;

			if (firstTs == 0)
				firstTs = VPkt.Header.Timestamp;

			LOG_V("RTSP AUDIO TS %lu BYTES %lu\n", APkt.Header.Timestamp, APkt.Header.DataSize + sizeof(PacketHeader));
			bool success = IsThreadRunning && !PacketizeLE16((char*)APkt.Data, APkt.Header.DataSize, (APkt.Header.Timestamp - firstTs) / 1000, RTSP_LE16SendPacket);

			if (!success)
				break;
		}
	}
}

static void RTSP_Init()
{
	RTP_InitializeSequenceNumbers();
	LaunchThread(&RTSPThread, RTSP_ServerThread, NULL, Buffers.RTSPMode.ServerThreadStackArea, sizeof(Buffers.RTSPMode.ServerThreadStackArea), 0x2D);
}

static void RTSP_Exit()
{
	RTSP_StopServer();
	JoinThread(&RTSPThread);
}

const StreamMode RTSP_MODE = { RTSP_Init, RTSP_Exit, RTSP_StreamVideo, RTSP_StreamAudio, NULL, NULL };
#endif