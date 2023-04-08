#if !defined(USB_ONLY)
#include "modes.h"
#include "../net/sockets.h"
#include "../capture.h"

static inline int TCP_BeginListen(GrcStream stream)
{
	LOG("TCP %d Begin listen\n", (int)stream);
	if (stream == GrcStream_Video)
	{
		return SocketTcpListen(9911, true);
	}
	else
	{
		return SocketTcpListen(9922, true);
	}
}

static inline int TCP_Accept(GrcStream stream)
{
restart:
	int listen = TCP_BeginListen(stream);

	while (IsThreadRunning)
	{
		int client = SocketTcpAccept(listen, NULL, NULL);
		if (client != SOCKET_INVALID)
		{
			LOG("TCP %d Accepted client\n", (int)stream);
			SocketClose(&listen);
			return client;
		}

		svcSleepThread(1E+9);
		if (SocketIsErrnoNetDown())
		{
			LOG("TCP %d Network change detected\n", (int)stream);
			SocketClose(&listen);
			goto restart;
		}
	}

	SocketClose(&listen);
	return SOCKET_INVALID;
}

typedef struct 
{
	GrcStream Type;
	ConsumerProducer* Target;
	PacketHeader* Pkt;
	const char* FullPacket;
} StreamConf;

const StreamConf VideoConfig = {
	GrcStream_Video,
	&VideoProducer,
	&VPkt.Header,
	(const char*)&VPkt
};

const StreamConf AudioConfig = {
	GrcStream_Audio,
	&AudioProducer,
	&APkt.Header,
	(const char*)&APkt
};

static void TCP_StreamThread(void* argConfig)
{
	if (!IsThreadRunning)
		fatalThrow(ERR_TCP_VIDEO);

	StreamConf config = *(const StreamConf*)argConfig;

	LOG("TCP %d Thread started\n", (int)config.Type);

	while (IsThreadRunning) {
		int client = TCP_Accept(config.Type);
		if (client == SOCKET_INVALID) {
			LOG("TCP %d Accept failed\n", (int)config.Type);
			continue;
		}

		CaptureOnClientConnected(config.Target);

		while (true)
		{
			CaptureBeginConsume(config.Target);

			bool success = IsThreadRunning;

			if (success)
			{
				//LOG("Sending MAGIC %x TS %lu BYTES %lu\n", config.Pkt->Magic, config.Pkt->Timestamp, config.Pkt->DataSize + sizeof(PacketHeader));
				success = SocketSendAll(&client, config.FullPacket, config.Pkt->DataSize + sizeof(PacketHeader));
			}

			CaptureEndConsume(config.Target);

			if (!success)
			{
				LOG("TCP %d send failed %d %d\n", (int)config.Type, g_bsdErrno, IsThreadRunning);
				break;
			}
		}

		CaptureOnClientDisconnected(config.Target);

		LOG("TCP %d Closing client\n", (int)config.Type);
		SocketClose(&client);
		svcSleepThread(2E+8);
	}

	LOG("TCP %d Thread terminating\n", (int)config.Type);
}

static void TCP_Init() 
{
	VPkt.Header.Magic = STREAM_PACKET_MAGIC_VIDEO;
	APkt.Header.Magic = STREAM_PACKET_MAGIC_AUDIO;
}

const StreamMode TCP_MODE = { TCP_Init, NULL, TCP_StreamThread, TCP_StreamThread, (void*)&VideoConfig, (void*)&AudioConfig};

#endif