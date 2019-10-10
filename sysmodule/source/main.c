#include <stdlib.h>
#include <switch.h>
#include <pthread.h>

#define MODE_USB
//#define MODE_SOCKET

#if defined(MODE_USB) && defined(MODE_SOCKET)
#error Define only one between MODE_USB and MODE_SOCKET
#elif !defined(MODE_USB) && !defined(MODE_SOCKET)
#pragma message "No mode has been defined, dafaulting to MODE_USB"
#define MODE_USB
#endif

#if !defined(__SWITCH__)
//Silence visual studio errors
#define __attribute__(x) 
typedef u64 ssize_t;
#endif

void StopRecording();

extern u32 __start__;
u32 __nx_applet_type = AppletType_None;
#if defined(MODE_USB)
	#define INNER_HEAP_SIZE 0x80000
#else
	#define INNER_HEAP_SIZE 0x300000
#endif
size_t nx_inner_heap_size = INNER_HEAP_SIZE;
char nx_inner_heap[INNER_HEAP_SIZE];

void __libnx_initheap(void)
{
	void*  addr = nx_inner_heap;
	size_t size = nx_inner_heap_size;

	// Newlib
	extern char* fake_heap_start;
	extern char* fake_heap_end;

	fake_heap_start = (char*)addr;
	fake_heap_end   = (char*)addr + size;
}

void __attribute__((weak)) __appInit(void)
{
	svcSleepThread(2E+10);

	Result rc;

	rc = smInitialize();
	if (R_FAILED(rc))
		fatalSimple(MAKERESULT(Module_Libnx, LibnxError_InitFail_SM));

	rc = fsInitialize();
	if (R_FAILED(rc))
		fatalSimple(MAKERESULT(Module_Libnx, LibnxError_InitFail_FS));

	rc = setsysInitialize();
    if (R_SUCCEEDED(rc)) {
        SetSysFirmwareVersion fw;
        rc = setsysGetFirmwareVersion(&fw);
        if (R_SUCCEEDED(rc))
            hosversionSet(MAKEHOSVERSION(fw.major, fw.minor, fw.micro));
        setsysExit();
    }
	
	if (R_FAILED(rc))
		fatalSimple(MAKERESULT(1, 10));
	
	fsdevMountSdmc();
}

void __attribute__((weak)) __appExit(void)
{
	fsdevUnmountAll();
	fsExit();
#if defined(MODE_USB)
	usbCommsExit();
#else
	socketExit();
#endif
	StopRecording();
	smExit();
}


const int VbufSz = 0x32000;
const int AbufSz = 0x1000;
#if defined(MODE_USB) 
const int AudioBatchSz = 6;
#else 
const int AudioBatchSz = 12;
#endif

u8* Vbuf = NULL;
u8* Abuf = NULL;
u32 VOutSz = 0;
u32 AOutSz = 0;

bool g_recInited = false;
void InitRecording() 
{
	if (g_recInited) return;

	Result res = grcdInitialize();
	if (R_FAILED(res))
		fatalSimple(MAKERESULT(1, 20));

	res = grcdBegin();
	if (R_FAILED(res))
		fatalSimple(MAKERESULT(1, 30));

	Vbuf = aligned_alloc(0x1000, VbufSz);
	if (!Vbuf)
		fatalSimple(MAKERESULT(1, 40));

	Abuf = aligned_alloc(0x1000, AbufSz * AudioBatchSz);
	if (!Vbuf)
		fatalSimple(MAKERESULT(1, 50));

	g_recInited = true;
}

void StopRecording()
{
	if (!g_recInited) return;
	grcdExit();
	free(Vbuf);
	free(Abuf);
	g_recInited = false;
}

//Batch sending audio samples to improve speed
static void ReadAudioStream()
{
	u32 unk = 0;
	u64 timestamp = 0;
	u32 TmpAudioSz = 0;
	AOutSz = 0;
	for (int i = 0; i < AudioBatchSz; i++)
	{
		Result res = grcdRead(GrcStream_Audio, Abuf + AOutSz, AbufSz, &unk, &TmpAudioSz, &timestamp);
		if (R_FAILED(res) || TmpAudioSz <= 0)
		{
			--i;
			continue;
		}
		AOutSz += TmpAudioSz;
	}
}

static void ReadVideoStream()
{
	u32 unk = 0;
	u64 timestamp = 0;

	while (true) {
		Result res = grcdRead(GrcStream_Video, Vbuf, VbufSz, &unk, &VOutSz, &timestamp);
		if (R_FAILED(res) || VOutSz <= 0)
		{
			VOutSz = 0;
			svcSleepThread(5000000);
			continue;
		}
		break;
	}
}

#if defined(MODE_USB)
const u32 VideoStream = 0;
const u32 AudioStream = 1;
const u32 REQMAGIC_VID = 0xAAAAAAAA;
const u32 REQMAGIC_AUD = 0xBBBBBBBB;

static u32 WaitForInputReq(u32 dev)
{
	while (true)
	{
		u32 initSeq = 0;
		if (usbCommsReadEx(&initSeq, sizeof(initSeq), dev) == sizeof(initSeq))
			return initSeq;
	}
	return 0;
}

static void SendStream(GrcStream stream, u32 Dev)
{
	u32* size = stream == GrcStream_Video ? &VOutSz : &AOutSz;
	 
	if (*size <= 0)
	{
		*size = 0;
		usbCommsWriteEx(size, sizeof(*size), Dev);
	}

	u8* TargetBuf = stream == GrcStream_Video ? Vbuf : Abuf;

	if (usbCommsWriteEx(size, sizeof(*size), Dev) != sizeof(*size)) return;
	if (usbCommsWriteEx(TargetBuf, *size, Dev) != *size) return;
	return;
}

void* StreamThreadMain(void* _stream)
{
	GrcStream stream = (GrcStream)_stream;
	const u32 Dev = stream == GrcStream_Video ? VideoStream : AudioStream;
	u8 ErrorCode = stream == GrcStream_Video ? 70 : 80;
	u32 ThreadMagic = stream == GrcStream_Video ? REQMAGIC_VID : REQMAGIC_AUD;

	void (*ReadStreamFn)() = stream == GrcStream_Video ? ReadVideoStream : ReadAudioStream;

	while (true)
	{
		u32 cmd = WaitForInputReq(Dev);

		if (cmd == ThreadMagic)
		{
			ReadStreamFn();
			SendStream(stream, Dev);
		}
		else fatalSimple(MAKERESULT(1, ErrorCode));
	}
	return NULL;
}
#else
#ifdef __SWITCH__
#include <unistd.h>
#include <arpa/inet.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <fcntl.h>
#else
#include <WinSock2.h>
#endif

int VideoSock = -1;
int AudioSock = -1;

Result CreateSocket(int *OutSock, int port, int baseError)
{
	int err = 0, sock = -1;
	struct sockaddr_in temp;

	sock = socket(AF_INET, SOCK_STREAM, 0);
	if (sock < 0)
		return MAKERESULT(baseError, 1);
	
	temp.sin_family = AF_INET;
	temp.sin_addr.s_addr = INADDR_ANY;
	temp.sin_port = htons(port);

	const int optVal = 1;
	err = setsockopt(sock, SOL_SOCKET, SO_REUSEADDR, (void*)&optVal, sizeof(optVal));
	if (err)
		return MAKERESULT(baseError, 2);

	err = bind(sock, (struct sockaddr*) & temp, sizeof(temp));
	if (err)
		return MAKERESULT(baseError, 3);

	err = listen(sock, 1);
	if (err)
		return MAKERESULT(baseError, 4);

	*OutSock = sock;
	return 0;
}

Result SocketInit(GrcStream stream)
{
	Result rc;
	if (stream == GrcStream_Video)
	{
		rc = CreateSocket(&VideoSock, 6666, 2);
		if (R_FAILED(rc)) return rc;
	}
	else 
	{
		rc = CreateSocket(&AudioSock, 6667, 3);
		if (R_FAILED(rc)) return rc;
	}
	return 0;
}

int AudioCurSock = -1;
int VideoCurSock = -1;

Result SocketReset(GrcStream stream)
{
	if (stream == GrcStream_Video)
	{
		if (VideoSock != -1) close(VideoSock);
		if (VideoCurSock != -1) close(VideoCurSock);
		VideoSock = -1;
		VideoCurSock = -1;
	}
	else 
	{
		if (AudioSock != -1) close(AudioSock);
		if (AudioCurSock != -1) close(AudioCurSock);
		AudioSock = -1;
		AudioCurSock = -1;
	}

	return SocketInit(stream);
}

const u8 SPS[] = { 0x00, 0x00, 0x00, 0x01, 0x67, 0x64, 0x0C, 0x20, 0xAC, 0x2B, 0x40, 0x28, 0x02, 0xDD, 0x35, 0x01, 0x0D, 0x01, 0xE0, 0x80 };
const u8 PPS[] = { 0x00, 0x00, 0x00, 0x01, 0x68, 0xEE, 0x3C, 0xB0 };

void* StreamThreadMain(void* _stream)
{
	GrcStream stream = (GrcStream)_stream;
	int sock = stream == GrcStream_Video ? VideoSock : AudioSock;
	void (*ReadStreamFn)() = stream == GrcStream_Video ? ReadVideoStream : ReadAudioStream;

	u32* size = stream == GrcStream_Video ? &VOutSz : &AOutSz;
	u8* TargetBuf = stream == GrcStream_Video ? Vbuf : Abuf;

	while (true) {
		int curSock = accept(sock, 0, 0);

		int* OutSock = stream == GrcStream_Video ? &VideoCurSock : &AudioCurSock;
		*OutSock = curSock;

		if (curSock >= 0)
		{
			if (stream == GrcStream_Video) {
				write(curSock, SPS, sizeof(SPS));
				write(curSock, PPS, sizeof(PPS));
			}

			while (true)
			{
				ReadStreamFn();
				if (write(curSock, TargetBuf, *size) < 0)
					break;
			}
		}
		
		svcSleepThread(1E+9);
		SocketReset(stream);
		svcSleepThread(1E+9);
	}
	return NULL;
}
#endif


int main(int argc, char* argv[])
{
#if defined(MODE_USB)
	if (R_FAILED(usbCommsInitializeEx(2, NULL)))
		fatalSimple(MAKERESULT(1, 60));
#else
	Result rc = socketInitializeDefault();
	if (R_FAILED(rc)) fatalSimple(rc);
	rc = SocketInit(GrcStream_Video);
	if (R_FAILED(rc)) fatalSimple(rc);
	rc = SocketInit(GrcStream_Audio);
	if (R_FAILED(rc)) fatalSimple(rc);
#endif

	InitRecording();

#if defined(MODE_USB)
	//TODO: why does accept() on the main thread block the audioThread ?
	pthread_t audioThread;
	if (pthread_create(&audioThread, NULL, StreamThreadMain, (void*)GrcStream_Audio))
		fatalSimple(MAKERESULT(1, 90));
#endif
	StreamThreadMain((void*)GrcStream_Video);

	//void* dummy;
	//pthread_join(audioThread, &dummy);

    return 0;
}
