#if !defined(USB_ONLY)
#include "modes.h"
#include "../net/sockets.h"
#include "../capture.h"

static inline int TCP_BeginListen(GrcStream stream)
{
	LOG("TCP %d Begin listen\n", (int)stream);
	if (stream == GrcStream_Video)
	{
		return SocketTcpListen(9911);
	}
	else
	{
		return SocketTcpListen(9922);
	}
}

static inline int TCP_Accept(GrcStream stream)
{
restart:
	int listen = TCP_BeginListen(stream);

	if (listen == SOCKET_INVALID)
	{
		LOG("TCP %d Listen failed\n", (int)stream);
		svcSleepThread(1E+9);
		return SOCKET_INVALID;
	}

	while (IsThreadRunning)
	{
		int client = SocketTcpAccept(listen, NULL, NULL);
		if (client != SOCKET_INVALID)
		{
			LOG("TCP %d Accepted client %d\n", (int)stream, client);
			SocketClose(&listen);
			return client;
		}

		svcSleepThread(1E+9);
		if (SocketIsListenNetDown())
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
		fatalThrow(ERR_TCP_THREAD);

	StreamConf config = *(const StreamConf*)argConfig;

	LOG("TCP %d Thread started\n", (int)config.Type);

	while (IsThreadRunning) {
		int client = TCP_Accept(config.Type);
		if (client == SOCKET_INVALID) {
			LOG("TCP %d Accept failed\n", (int)config.Type);
			continue;
		}

		// Give the client a few moments to be ready
		svcSleepThread(5E+8);

		u64 total = 0;
		CaptureOnClientConnected(config.Target);

		while (IsThreadRunning)
		{
			if (!CaptureWaitProduced(config.Target))
				continue;

			LOG_V("Sending MAGIC %x TS %lu BYTES %lu\n", config.Pkt->Magic, config.Pkt->Timestamp, config.Pkt->DataSize + sizeof(PacketHeader));
			bool success = SocketSendAll(client, config.FullPacket, config.Pkt->DataSize + sizeof(PacketHeader));

			if (!success)
			{
				LOG("TCP %d send failed %d %d\n", (int)config.Type, g_bsdErrno, IsThreadRunning);
				break;
			}

#if LOGGING_ENABLED
			if (success)
				total += config.Pkt->DataSize + sizeof(PacketHeader);
#else
			(void)total;
#endif

			CaptureSignalConsumed(config.Target);
		}

		CaptureOnClientDisconnected(config.Target);

		LOG("TCP %d Closing client after %lu bytes\n", (int)config.Type, total);
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

const StreamMode TCP_MODE = { TCP_Init, NULL, TCP_StreamThread, TCP_StreamThread, (void*)&VideoConfig, (void*)&AudioConfig };

#endif