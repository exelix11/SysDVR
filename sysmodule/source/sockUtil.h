#pragma once
#include <switch.h>
#ifdef __SWITCH__
#include <unistd.h>
#include <arpa/inet.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <fcntl.h>
#include <errno.h>
#else
//not actually used, just to stop visual studio from complaining.
//~~i regret nothing~~
#define F_SETFL 1
#define O_NONBLOCK 1
#define F_GETFL 1
#define MSG_DONTWAIT 0
#include <WinSock2.h>
#endif

Result CreateTCPListener(int* OutSock, int port, int baseError, bool LocalOnly);