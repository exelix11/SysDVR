#ifndef USB_ONLY
#include <string.h>
#include <fcntl.h> // fcntl

#include "sockets.h"
#define NX_EAGAIN 11
#define NX_O_NONBLOCK 0x800

#include "../modes/defines.h"
#include "../modes/modes.h"
#include "../capture.h"

// We want to statically allocate all socketing resources to avoid heap fragmentation
// This means we use the bsd service API directly instead of the libnx wrapper

static bool SocketReady;

// Libnx defaults but tweaked for our use case
#define TCP_TX_SZ 0x44000
#define TCP_RX_SZ 0x1000

#define UDP_TX_SZ 0x2000
#define UDP_RX_SZ 0x1000

#define SOCK_EFFICIENCY 2

// Formula taken from libnx itslf
#define PAGE_ALIGN(x) ((x + 0xFFF) &~ 0xFFF)
#define TMEM_SIZE PAGE_ALIGN(TCP_TX_SZ + TCP_RX_SZ + UDP_TX_SZ + UDP_RX_SZ) * SOCK_EFFICIENCY

static u8 alignas(0x1000) TmemBackingBuffer[TMEM_SIZE];

void SocketInit()
{
	if (SocketReady)
		return;

	memset(TmemBackingBuffer, 0, sizeof(TmemBackingBuffer));

	const BsdInitConfig config = {
		.version = 1,
		.tmem_buffer = TmemBackingBuffer,
		.tmem_buffer_size = TMEM_SIZE,

		.tcp_tx_buf_size = 0x4000,
		.tcp_rx_buf_size = 0x1000,
		.tcp_tx_buf_max_size = TCP_TX_SZ,
		.tcp_rx_buf_max_size = TCP_RX_SZ,

		.udp_tx_buf_size = UDP_TX_SZ,
		.udp_rx_buf_size = UDP_RX_SZ,

		.sb_efficiency = SOCK_EFFICIENCY
	};

	LOG("Initializing BSD\n");
	R_THROW(bsdInitialize(&config, 2, BsdServiceType_Auto));
}

void SocketDeinit()
{
	if (!SocketReady)
		return;

	LOG("Exiting BSD\n");
	bsdExit();
}

void SocketClose(int* socket)
{
	if (*socket != SOCKET_INVALID)
	{
		bsdClose(*socket);
		*socket = SOCKET_INVALID;
	}
}

int SocketUdp()
{
	return bsdSocket(AF_INET, SOCK_DGRAM, 0);
}

int SocketTcpListen(short port, bool blocking)
{
	while (true) {
		int socket = bsdSocket(AF_INET, SOCK_STREAM, 0);
		if (socket < 0)
			goto failed;

		if (!blocking)
		{
			if (!SocketMakeNonBlocking(socket))
				goto failed;
		}

		const int optVal = 1;
		if (bsdSetSockOpt(socket, SOL_SOCKET, SO_REUSEADDR, (void*)&optVal, sizeof(optVal)) == -1)
			goto failed;

		struct sockaddr_in addr;
		addr.sin_family = AF_INET;
		addr.sin_addr.s_addr = INADDR_ANY;
		addr.sin_port = htons(port);

		if (bsdBind(socket, (struct sockaddr*)&addr, sizeof(addr)) == -1)
			goto failed;

		if (bsdListen(socket, 1) == -1)
			goto failed;

		return socket;

	failed:
		LOG("Failed");

		if (socket != SOCKET_INVALID)
			bsdClose(socket);

		svcSleepThread(1);
		continue;
	}
}

// This is a weird hack, we need to figure out when the console is in sleep mode
// and reset the listening socket when it wakes up, the only way i found to get
// a meaningful error code is from poll, which will return 0 when no connection is pending
// and 1 otherwise but if Accept fails with EAGAIN, we know the console was in sleep mode.
// We should use nifm but that comes with its share of weirdness.....
bool SocketIsErrnoNetDown()
{
	return g_bsdErrno == NX_EAGAIN;
}

int SocketTcpAccept(int listenerHandle, struct sockaddr* addr, socklen_t* addrlen)
{
	struct pollfd pollinfo;
	pollinfo.fd = listenerHandle;
	pollinfo.events = POLLIN;
	pollinfo.revents = 0;

	int rc = bsdPoll(&pollinfo, 1, 0);
	if (rc > 0)
	{
		if (pollinfo.revents & POLLIN)
		{
			return bsdAccept(listenerHandle, addr, addrlen);
		}
	}

	return SOCKET_INVALID;
}

bool SocketUDPSendTo(int socket, const void* data, u32 size, struct sockaddr* addr, socklen_t addrlen)
{
	return bsdSendTo(socket, data, size, 0, addr, addrlen) == size;
}

bool SocketSendAll(int* socket, const void* buffer, u32 size)
{
	u32 sent = 0;
	while (sent < size)
	{
		int sock = *socket;
		if (sock == SOCKET_INVALID)
			return false;

		int res = bsdSend(sock, (const char*)buffer + sent, size - sent, 0);
		if (res == -1)
		{
			if (g_bsdErrno == NX_EAGAIN)
				svcSleepThread(1);
			else
				return false;
		}
		sent += res;
	}

	return true;
}

s32 SocketRecv(int* socket, void* buffer, u32 size)
{
	ssize_t r = bsdRecv(*socket, buffer, size, MSG_DONTWAIT);
	if (r == -1)
	{
		if (g_bsdErrno == NX_EAGAIN)
			return 0;
		else
			return -1;
	}
	return (s32)r;
}

bool SocketMakeNonBlocking(int socket)
{
	return bsdFcntl(socket, F_SETFL, NX_O_NONBLOCK) != -1;
}

#endif