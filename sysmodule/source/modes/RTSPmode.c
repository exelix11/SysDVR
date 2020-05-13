#if !defined(USB_ONLY)
#include "modes.h"
#include "../grcd.h"
#include "../rtsp/RTSP.h"
#include "../rtsp/H264Packetizer.h"
#include "../rtsp/LE16Packetizer.h"

static Thread RTSPThread;

static void RTSP_StreamVideo(void* _)
{
	if (!IsThreadRunning)
		fatalThrow(ERR_RTSP_VIDEO);

	while (IsThreadRunning)
	{
		while (!RTSP_ClientStreaming && IsThreadRunning) svcSleepThread(1E+8); // 1/10 of second
		if (!IsThreadRunning) break;
		
		while (true)
		{			
			if (!ReadVideoStream()) 
				TerminateOrContinue
			
			int error = PacketizeH264((char*)VPkt.Data, VPkt.Header.DataSize, VPkt.Header.Timestamp / 1000, RTSP_H264SendPacket);			
			if (error) break;
		}
	}
}

static void RTSP_StreamAudio(void* _)
{
	if (!IsThreadRunning)
		fatalThrow(ERR_RTSP_AUDIO);

	while (IsThreadRunning)
	{
		while (!RTSP_ClientStreaming && IsThreadRunning) svcSleepThread(1E+8); // 1/10 of second
		if (!IsThreadRunning) break;

		while (true)
		{			
			if (!ReadAudioStream()) 
				TerminateOrContinue

			int error = PacketizeLE16((char*)APkt.Data, APkt.Header.DataSize, APkt.Header.Timestamp / 1000, RTSP_LE16SendPacket);
			if (error) break;
		}
	}
}

static void RTSP_Init()
{
	RTP_InitializeSequenceNumbers();
	LaunchThread(&RTSPThread, RTSP_ServerThread);
}

static void RTSP_Exit()
{
	RTSP_StopServer();
	JoinThread(&RTSPThread);
}

StreamMode RTSP_MODE = { RTSP_Init, RTSP_Exit, RTSP_StreamVideo, RTSP_StreamAudio };
#endif