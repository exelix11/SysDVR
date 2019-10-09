#include <stdlib.h>
#include <switch.h>
#include <pthread.h>

#if !defined(__SWITCH__)
//Silence visual studio errors
#define __attribute__(x) 
typedef u64 ssize_t;
#endif

void StopRecording();

extern u32 __start__;
u32 __nx_applet_type = AppletType_None;
#define INNER_HEAP_SIZE 0x80000
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
	svcSleepThread(1E+10);

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
	usbCommsExit();
	StopRecording();
	smExit();
}

const u32 VideoStream = 0;
const u32 AudioStream = 1;

const int VbufSz = 0x32000;
const int AbufSz = 0x1000;
const int AudioBatchSz = 12;
const u32 REQMAGIC_VID = 0xAAAAAAAA;
const u32 REQMAGIC_AUD = 0xBBBBBBBB;
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

int main(int argc, char* argv[])
{
	if (R_FAILED(usbCommsInitializeEx(2, NULL)))
		fatalSimple(MAKERESULT(1, 60));

	InitRecording();

	pthread_t audioThread;
	if (pthread_create(&audioThread, NULL, StreamThreadMain, (void*)GrcStream_Audio))
		fatalSimple(MAKERESULT(1, 90));

	StreamThreadMain((void*)GrcStream_Video);

    return 0;
}
