#if !defined(USB_ONLY)
#include "ipc.h"
#include <string.h>
#include "../modes/defines.h"

// From main
extern void SetModeID(u32 mode);
extern u32 GetCurrentMode();
extern bool CanChangeMode();

static Handle handles[2];
static SmServiceName serverName;

static Handle* const serverHandle = &handles[0];
static Handle* const clientHandle = &handles[1];

static bool isClientConnected = false;

#define R_THROW(x) do { Result r = x; if (R_FAILED(r)) { fatalThrow(r); }  } while(0)

static void StartServer()
{
	isClientConnected = false;
	memcpy(serverName.name, "sysdvr", sizeof("sysdvr"));
	R_THROW(smRegisterService(serverHandle, serverName, false, 1));
}

static void StopServer()
{
	svcCloseHandle(*serverHandle);
	R_THROW(smUnregisterService(serverName));
}

typedef struct {
	u32 type;
	u64 cmdId;
} Request;

static Request ParseRequestFromTLS()
{
	Request res = {0};

	void* base = armGetTls();
	HipcParsedRequest hipc = hipcParseRequest(base);

	res.type = hipc.meta.type;

	if (hipc.meta.type == CmifCommandType_Request)
	{
		CmifInHeader* header = (CmifInHeader*)cmifGetAlignedDataStart(hipc.data.data_words, base);
		size_t dataSize = hipc.meta.num_data_words * 4;

		if (!header)
			fatalThrow(ERR_IPC_INVHEADER);
		if (dataSize < sizeof(CmifInHeader))
			fatalThrow(ERR_IPC_INVSIZE);
		if (header->magic != CMIF_IN_HEADER_MAGIC)
			fatalThrow(ERR_IPC_INVMAGIC);

		res.cmdId = header->command_id;
	}

	return res;
}

static void WriteResponseToTLS(Result rc)
{
	HipcMetadata meta = { 0 };
	meta.type = CmifCommandType_Request;
	meta.num_data_words = (sizeof(CmifOutHeader) + 0x10) / 4;

	void* base = armGetTls();
	HipcRequest hipc = hipcMakeRequest(base, meta);
	CmifOutHeader* rawHeader = (CmifOutHeader*)cmifGetAlignedDataStart(hipc.data_words, base);

	rawHeader->magic = CMIF_OUT_HEADER_MAGIC;
	rawHeader->result = rc;
	rawHeader->token = 0;
}

static void WritePayloadResponseToTLS(Result rc, const void* payload, u32 len)
{
	HipcMetadata meta = { 0 };
	meta.type = CmifCommandType_Request;
	meta.num_data_words = (sizeof(CmifOutHeader) + 0x10 + len) / 4;

	void* base = armGetTls();
	HipcRequest hipc = hipcMakeRequest(base, meta);
	CmifOutHeader* rawHeader = (CmifOutHeader*)cmifGetAlignedDataStart(hipc.data_words, base);

	rawHeader->magic = CMIF_OUT_HEADER_MAGIC;
	rawHeader->result = rc;
	rawHeader->token = 0;

	memcpy(((u8*)rawHeader) + sizeof(CmifOutHeader), payload, len);
}

static u32 modeToSet = TYPE_MODE_ERROR;
static void ApplyModeChanges()
{
	if (modeToSet == TYPE_MODE_ERROR)
		return;

	SetModeID(CMD_SET_TO_MODE(modeToSet));

	modeToSet = TYPE_MODE_ERROR;
}

static bool HandleCommand(u64 id)
{
	switch (id)
	{
		case CMD_GET_VER:
		{
			u32 ver = SYSDVR_VERSION;
			WritePayloadResponseToTLS(0, &ver, sizeof(ver));
			return false;
		}
		case CMD_GET_MODE:
		{
			u32 mode = GetCurrentMode();
			WritePayloadResponseToTLS(0, &mode, sizeof(mode));
			return false;
		}
		case CMD_SET_USB:
		case CMD_SET_TCP:
		case CMD_SET_RTSP:
		case CMD_SET_OFF:
			if (CanChangeMode())
			{
				modeToSet = id;
				WriteResponseToTLS(0);
			}
			else WriteResponseToTLS(ERR_MAIN_SWITCHING);
			return false;
		default:
			WriteResponseToTLS(ERR_IPC_UNKCMD);
			return true;
	}
}

static void WaitAndProcessRequest()
{
	s32 index = -1;

	R_THROW(svcWaitSynchronization(&index, handles, isClientConnected ? 2 : 1, UINT64_MAX));

	if (index == 0)
	{
		Handle newcli;
		// Accept session
		R_THROW(svcAcceptSession(&newcli, *serverHandle));
		
		// Max clients reached
		if (isClientConnected)
			R_THROW(svcCloseHandle(newcli));

		isClientConnected = true;
		*clientHandle = newcli;
	}
	else if (index == 1)
	{
		// Handle message
		if (!isClientConnected) {
			fatalThrow(ERR_IPC_NOCLIENT);
		}

		s32 _idx;
		R_THROW(svcReplyAndReceive(&_idx, clientHandle, 1, 0, UINT64_MAX));

		bool shouldClose = false;
		Request r = ParseRequestFromTLS();
		switch (r.type)
		{
		case CmifCommandType_Request:
			shouldClose = HandleCommand(r.cmdId);
			break;
		case CmifCommandType_Close:
			WriteResponseToTLS(0);
			shouldClose = true;
			break;
		default:
			WriteResponseToTLS(ERR_HIPC_UNKREQ);
			break;
		}

		Result rc = svcReplyAndReceive(&_idx, clientHandle, 0, *clientHandle, 0);

		if (rc != KERNELRESULT(TimedOut))
			R_THROW(rc);

		if (shouldClose) {
			R_THROW(svcCloseHandle(*clientHandle));
			isClientConnected = false;
			ApplyModeChanges();
		}
	}
	else return;
}

void IpcThread()
{
	StartServer();

	while (1)
		WaitAndProcessRequest();

	StopServer();
}
#endif