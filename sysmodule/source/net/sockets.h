#pragma once
#include <switch.h>
#include <arpa/inet.h> // htons
#include <switch/services/bsd.h>

#define SOCKET_INVALID -1

void SocketInit();
void SocketDeinit();

int SocketUdp();

int SocketTcpListen(short port);

// Only valid after SocketTcpAccept() returns SOCKET_INVALID
bool SocketIsErrnoNetDown();

// Returns SOCKET_INVALID on error, use SocketIsErrnoNetDown() to check if the listener needs to be reset
int SocketTcpAccept(int listenerHandle, struct sockaddr* addr, socklen_t* addrlen);

// Coses a socket, also sets it to SOCKET_INVALID
void SocketClose(int* socket);

// Returns true on success
bool SocketSendAll(int socket, const void* buffer, u32 size);

// Returns true on success
bool SocketUDPSendTo(int socket, const void* data, u32 size, struct sockaddr* addr, socklen_t addrlen);

// Returns 0 on timeout, -1 on error, otherwise the number of bytes received
s32 SocketRecv(int socket, void* buffer, u32 size);

bool SocketMakeNonBlocking(int socket);

// Hopefully heps saving memory by getting rid of receiving buffers
void SocketCloseReceivingEnd(int socket);