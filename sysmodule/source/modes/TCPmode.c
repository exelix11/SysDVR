#if !defined(USB_ONLY)
#include "modes.h"
#include "../net/sockets.h"
#include "../capture.h"

static int UdpAdvertiseSocket = SOCKET_INVALID;

static struct sockaddr_in UdpBroadcastAddr = {
	.sin_family = AF_INET,
};

static void InitBroadcast(GrcStream stream)
{
	// Only one thread need to do the advertisement
	if (stream != GrcStream_Video)
		return;

	UdpBroadcastAddr.sin_port = htons(19999);
	UdpBroadcastAddr.sin_addr.s_addr = SocketGetBroadcastAddress();

	UdpAdvertiseSocket = SocketUdp();
	if (!SocketSetBroadcast(UdpAdvertiseSocket, true))
		LOG("UDP set broadcast failed: %d\n", SocketNativeErrno());
}

static void DeinitBroadcast(GrcStream stream)
{
	if (stream != GrcStream_Video)
		return;

	SocketClose(&UdpAdvertiseSocket);
}

static inline int TCP_BeginListen(GrcStream stream)
{
	int port = stream == GrcStream_Video ? 9911 : 9922;
	LOG("TCP %d Begin listen on %d\n", (int)stream, port);
	InitBroadcast(stream);
	return SocketTcpListen(port);
}

static inline int TCP_Accept(GrcStream stream)
{
	bool advertise = false;
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
			DeinitBroadcast(stream);
			return client;
		}

		svcSleepThread(1E+9);

		// Advertise every 2 seconds
		if (stream == GrcStream_Video && advertise)
		{
			advertise = false;
			if (UdpAdvertiseSocket != SOCKET_INVALID)
			{
				LOG("Sending UDP advertisement broadcast\n");
				if (!SocketUDPSendTo(UdpAdvertiseSocket, SysDVRBeacon, SysDVRBeaconLen, (struct sockaddr*)&UdpBroadcastAddr, sizeof(UdpBroadcastAddr)))
					LOG("UDP advertisement failed: %d\n", SocketNativeErrno());
			}
		}
		else advertise = true;

		if (SocketIsListenNetDown())
		{
			LOG("TCP %d Network change detected\n", (int)stream);
			SocketClose(&listen);
			DeinitBroadcast(stream);
			goto restart;
		}
	}

	SocketClose(&listen);
	DeinitBroadcast(stream);

	return SOCKET_INVALID;
}

typedef struct
{
	GrcStream Type;
	PacketHeader* Pkt;
	const char* FullPacket;
} StreamConf;

const StreamConf VideoConfig = {
	GrcStream_Video,
	&VPkt.Header,
	(const char*)&VPkt
};

const StreamConf AudioConfig = {
	GrcStream_Audio,
	&APkt.Header,
	(const char*)&APkt
};

typedef bool (*StreamReadFunc)();

static void TCP_StreamThread(void* argConfig)
{
	if (!IsThreadRunning)
		fatalThrow(ERR_TCP_THREAD);

	StreamConf config = *(const StreamConf*)argConfig;
	const StreamReadFunc ReadStream = config.Type == GrcStream_Video ? CaptureReadVideo : CaptureReadAudio;

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
		while (true)
		{
			if (!ReadStream() || !IsThreadRunning)
				break;

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
		}

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

const StreamMode TCP_MODE = {
	TCP_Init, NULL,
	TCP_StreamThread, TCP_StreamThread,
	(void*)&VideoConfig, (void*)&AudioConfig,
	1
};

#endif