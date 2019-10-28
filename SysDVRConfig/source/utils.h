#pragma once
#include <string.h>
#include <stdio.h>
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
#define F_SETFL 1
#define O_NONBLOCK 1
#define F_GETFL 1
#include <WinSock2.h>
#endif

bool CreateDummyFile(const char* fname);
bool FileExists(const char* fname);

#define ERR_SOCK -1
#define ERR_CONNECT -2
int ConnectToSysmodule();

#if !defined(RELEASE)
extern const u64 kHelper[];
extern u8 KhlpIndex;
extern u8 KhlpMax;
void PrintBuffer(const char *);
extern const char H264_testBuf[];
#endif