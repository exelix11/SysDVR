#if !defined(USB_ONLY)
#include "modes.h"
#include "../grcd.h"
#include "../rtsp/RTSP.h"
#include "../rtsp/H264Packetizer.h"
#include "../rtsp/LE16Packetizer.h"

#include <pthread.h>
static pthread_t RTSPThread;

static void* RTSP_StreamThreadMain(void* _stream)
{
	if (!IsThreadRunning)
		fatalSimple(MAKERESULT(1, 14));

	const GrcStream stream = (GrcStream)_stream;
	while (IsThreadRunning)
	{
		while (!RTSP_ClientStreaming && IsThreadRunning) svcSleepThread(1E+8); // 1/10 of second
		if (!IsThreadRunning) break;

		RTP_InitializeSequenceNumbers();
		while (true)
		{
			int error = 0;
			if (stream == GrcStream_Video)
			{
				static int SendPPS = 0;

				ReadVideoStream();
				//Not needed for interleaved RTSP but mpv seems to need it for UDP.
				if (++SendPPS > 100)
				{
					PacketizeH264((char*)SPS, sizeof(SPS), VTimestamp / 1000, RTSP_H264SendPacket);
					PacketizeH264((char*)PPS, sizeof(PPS), VTimestamp / 1000, RTSP_H264SendPacket);
					SendPPS = 0;
				}
				error = PacketizeH264((char*)Vbuf, VOutSz, VTimestamp / 1000, RTSP_H264SendPacket);
			}
			else
			{
				ReadAudioStream();
				error = PacketizeLE16((char*)Abuf, AOutSz, ATimestamp / 1000, RTSP_LE16SendPacket);
			}
			if (error) break;
		}
	}

	pthread_exit(NULL);
	return NULL;
}

static void RTSP_Init()
{
	pthread_create(&RTSPThread, NULL, RTSP_ServerThread, NULL);
}

static void RTSP_Exit()
{
	RTSP_StopServer();
	pthread_join(RTSPThread, NULL);
}

StreamMode RTSP_MODE = { RTSP_Init, RTSP_Exit, RTSP_StreamThreadMain };

#endif