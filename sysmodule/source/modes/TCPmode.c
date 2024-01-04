#if !defined(USB_ONLY)
#include "modes.h"
#include "../net/sockets.h"
#include "../capture.h"
#include "proto.h"

int g_tcpEnableBroadcast = 1;

// Prevent toggling this on and off during runtime or else we may leak the socket
// this can only be set by TCP_Init on mode switch
static int tcpSessionEnableBroadcast = 1;
static int UdpAdvertiseSocket = SOCKET_INVALID;

static struct sockaddr_in UdpBroadcastAddr = {
	.sin_family = AF_INET,
};

static void DeinitBroadcast(GrcStream stream)
{
	if (!tcpSessionEnableBroadcast)
		return;

	if (stream != GrcStream_Video)
		return;

	if (UdpAdvertiseSocket == SOCKET_INVALID)
		return;

	LOG("Closing UDP broadcast socket\n");
	SocketClose(&UdpAdvertiseSocket);
}

static void InitBroadcast(GrcStream stream)
{
	if (!tcpSessionEnableBroadcast)
		return;

	// Only one thread need to do the advertisement
	if (stream != GrcStream_Video)
		return;

	if (UdpAdvertiseSocket != SOCKET_INVALID)
		DeinitBroadcast(stream);

	UdpBroadcastAddr.sin_port = htons(19999);
	UdpBroadcastAddr.sin_addr.s_addr = SocketGetBroadcastAddress();

	LOG("Opening UDP broadcast socket\n");
	UdpAdvertiseSocket = SocketUdp();
	if (!SocketSetBroadcast(UdpAdvertiseSocket, true))
		LOG("UDP set broadcast failed: %d\n", SocketNativeErrno());
}

static inline void AdvertiseBroadcast(GrcStream stream)
{
	if (!tcpSessionEnableBroadcast)
		return;

	if (stream != GrcStream_Video)
		return;

	if (UdpAdvertiseSocket == SOCKET_INVALID)
	{
		InitBroadcast(stream);
	}
	
	LOG("Sending UDP advertisement broadcast\n");
	if (!SocketUDPSendTo(UdpAdvertiseSocket, SysDVRBeacon, SysDVRBeaconLen, (struct sockaddr*)&UdpBroadcastAddr, sizeof(UdpBroadcastAddr)))
	{
		LOG("UDP advertisement failed: %d\n", SocketNativeErrno());
		DeinitBroadcast(stream);
	}
}

static inline int TCP_BeginListen(GrcStream stream)
{
	int port = stream == GrcStream_Video ? 9911 : 9922;
	LOG("TCP %d Begin listen on %d\n", (int)stream, port);
	InitBroadcast(stream);
	return SocketTcpListen(port);
}

static inline bool TCP_DoHandshake(GrcStream stream, int socket)
{
	u8 buffer[PROTO_HANDSHAKE_SIZE];

	if (!SocketSendAll(socket, PROTO_HANDSHAKE_HELLO, sizeof(PROTO_HANDSHAKE_HELLO)))
		return false;

	if (!SocketRecevExact(socket, buffer, sizeof(buffer)))
		return false;

	// TCP threads are hardcoded to either video or audio
	ProtoHandshakeAccept accept = stream == GrcStream_Video ? 
		ProtoHandshakeAccept_Video : ProtoHandshakeAccept_Audio;

	ProtoParsedHandshake res = ProtoHandshake(accept, buffer, sizeof(buffer));

	if (!SocketSendAll(socket, &res.Result, sizeof(res.Result)))
		return false;

	return res.Result == Handshake_Ok;
}

static inline int TCP_Accept(GrcStream stream)
{
restart:
	bool advertise = false;
	int listen = TCP_BeginListen(stream);
	int ret = SOCKET_INVALID;

	if (listen == SOCKET_INVALID)
	{
		LOG("TCP %d Listen failed\n", (int)stream);
		goto leave;
	}

	while (IsThreadRunning)
	{
		// Advertise every 2 seconds
		if (stream == GrcStream_Video)
		{
			if (advertise)
				AdvertiseBroadcast(stream);
			advertise = !advertise;
		}

		int client = SOCKET_INVALID;
		SocketAcceptResult status = SocketTcpAccept(listen, &client, NULL, NULL);
		if (status == SocketAcceptError_OK)
		{
			LOG("TCP %d Got connection %d\n", (int)stream, client);
			
			if (TCP_DoHandshake(stream, client))
			{
				LOG("TCP %d Accepted client %d\n", (int)stream, client);
				ret = client;
				goto leave;
			}

			LOG("TCP %d Client %d handshake failed\n", (int)stream, client);
			SocketClose(&client);
		}
		else if (status == SocketAcceptError_NetDown)
		{
			LOG("TCP %d Network change detected\n", (int)stream);
			svcSleepThread(1E+9);
			SocketClose(&listen);
			DeinitBroadcast(stream);
			goto restart;
		}

		// reached when status == fail or status == ok but handshake failed
		svcSleepThread(1E+9);
	}

leave:
	SocketClose(&listen);
	DeinitBroadcast(stream);

	return ret;
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

		if (config.Type == GrcStream_Video)
			CaptureVideoConnected();
		else
			CaptureAudioConnected();

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

	DeinitBroadcast(config.Type);
	LOG("TCP %d Thread terminating\n", (int)config.Type);
}

static void TCP_Init()
{
	LOG("TCP Init\n");
	tcpSessionEnableBroadcast = g_tcpEnableBroadcast;
}

const StreamMode TCP_MODE = {
	TCP_Init, NULL,
	TCP_StreamThread, TCP_StreamThread,
	(void*)&VideoConfig, (void*)&AudioConfig
};

#endif