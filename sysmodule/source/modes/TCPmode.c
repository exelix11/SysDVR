#if !defined(USB_ONLY)
#include "modes.h"
#include "../capture.h"
#include "../sockUtil.h"

static inline int TCP_BeginListen(GrcStream stream)
{
	if (stream == GrcStream_Video)
	{
		return CreateTCPListener(9911, false, ERR_SOCK_TCP_1);
	}
	else
	{
		return CreateTCPListener(9922, false, ERR_SOCK_TCP_2);
	}
}

static inline int TCP_Accept(GrcStream stream)
{
	int listen = TCP_BeginListen(stream);

	/*
		This is needed because when resuming from sleep mode accept won't work anymore and errno value is not useful
		in detecting it, as we're using non-blocking mode the counter will reset the socket every 8 seconds
	*/
	int fails = 0;

	while (IsThreadRunning)
	{
		int client = accept(listen, NULL, NULL);
		if (client != -1)
		{
			close(listen);
			return client;
		}

		svcSleepThread(1E+9);
		if (++fails >= 8)
		{
			fails = 0;
			close(listen);
			listen = TCP_BeginListen(stream);
		}
	}

	close(listen);
	return -1;
}

static inline bool SendData(int sock, const PacketHeader* header, const char* FullPacket)
{
	u32 toSend = sizeof(PacketHeader) + header->DataSize; 
	int errorCount = 0;

	while (toSend)
	{
		int res = send(sock, FullPacket, toSend, 0);

		if (res < 0)
		{
			if (errno == EWOULDBLOCK || errno == EAGAIN)
			{
				// The client is not flushing the buffer fast enough, it probably disconnected
				if (++errorCount > 9)
					return false;

				svcSleepThread(1);
				continue;
			}
			else return false;
		}
		
		toSend -= res;
		FullPacket += res;
	}

	return true;
}

static void TCP_StreamThread(void* argStreamType)
{
	if (!IsThreadRunning)
		fatalThrow(ERR_TCP_VIDEO);

	const GrcStream type = (GrcStream)argStreamType;
	ConsumerProducer* const target = type == GrcStream_Video ? &VideoProducer : &AudioProducer;
	PacketHeader* const pkt = type == GrcStream_Video ? &VPkt.Header : &APkt.Header;

	while (IsThreadRunning) {
		int client = TCP_Accept(type);
		if (client < 0)
			continue;

		CaptureOnClientConnected(target);

		while (true)
		{
			CaptureBeginConsume(target);

			if (!IsThreadRunning)
				break;

			bool success = SendData(client, pkt, (const char*)pkt);

			CaptureEndConsume(target);

			if (!success)
				break;			
		}

		CaptureOnClientDisconnected(target);

		close(client);
		svcSleepThread(2E+8);
	}
}

static void TCP_Init() 
{
	VPkt.Header.Magic = STREAM_PACKET_MAGIC_VIDEO;
	APkt.Header.Magic = STREAM_PACKET_MAGIC_AUDIO;
}

static void TCP_Exit()
{

}

const StreamMode TCP_MODE = { TCP_Init, TCP_Exit, TCP_StreamThread, TCP_StreamThread, (void*)GrcStream_Video, (void*)GrcStream_Audio };

#endif