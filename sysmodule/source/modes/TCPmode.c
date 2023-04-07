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

	while (toSend)
	{
		int res = send(sock, FullPacket, toSend, 0);

		if (res < 0)
		{
			if (errno == EWOULDBLOCK || errno == EAGAIN)
			{
				if (!IsThreadRunning)
					return false;

				svcSleepThread(YieldType_WithoutCoreMigration);
				continue;
			}
			else return false;
		}
		
		toSend -= res;
		FullPacket += res;
	}

	return true;
}

typedef struct 
{
	GrcStream Type;
	ConsumerProducer* Target;
	PacketHeader* Pkt;
} StreamConf;

const StreamConf VideoConfig = {
	GrcStream_Video,
	&VideoProducer,
	&VPkt.Header
};

const StreamConf AudioConfig = {
	GrcStream_Audio,
	&AudioProducer,
	&APkt.Header
};

static void TCP_StreamThread(void* argConfig)
{
	if (!IsThreadRunning)
		fatalThrow(ERR_TCP_VIDEO);

	StreamConf config = *(const StreamConf*)argConfig;

	while (IsThreadRunning) {
		int client = TCP_Accept(config.Type);
		if (client < 0)
			continue;

		CaptureOnClientConnected(config.Target);

		while (true)
		{
			CaptureBeginConsume(config.Target);

			if (!IsThreadRunning)
				break;

			bool success = SendData(client, config.Pkt, (const char*)config.Pkt);

			CaptureEndConsume(config.Target);

			if (!success)
				break;			
		}

		CaptureOnClientDisconnected(config.Target);

		close(client);
		svcSleepThread(2E+8);
	}
}

static void TCP_Init() 
{
	VPkt.Header.Magic = STREAM_PACKET_MAGIC_VIDEO;
	APkt.Header.Magic = STREAM_PACKET_MAGIC_AUDIO;
}

const StreamMode TCP_MODE = { TCP_Init, NULL, TCP_StreamThread, TCP_StreamThread, (void*)&VideoConfig, (void*)&AudioConfig};

#endif