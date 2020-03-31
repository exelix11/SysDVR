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
#include <WinSock2.h>
#endif

int CreateTCPListener(int port, bool LocalOnly, int DebugFlag);