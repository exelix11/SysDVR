#if !defined(USB_ONLY)
#include <string.h>

#include "ipc.h"
#include "../modes/defines.h"
#include "../core.h"
#include "../util.h"

static Handle handles[2];
static SmServiceName serverName;

static Handle* const serverHandle = &handles[0];
static Handle* const clientHandle = &handles[1];

static void StartServer()
{
	*serverHandle = INVALID_HANDLE;
	*clientHandle = INVALID_HANDLE;
	
	memcpy(serverName.name, "sysdvr", sizeof("sysdvr"));
	R_THROW(smRegisterService(serverHandle, serverName, false, 1));
}

static void DisconnectClient()
{
	if (*clientHandle != INVALID_HANDLE)
	{
		svcCloseHandle(*clientHandle);
		*clientHandle = INVALID_HANDLE;
	}
}

static void StopServer()
{
	DisconnectClient();
	svcCloseHandle(*serverHandle);
	smUnregisterService(serverName);
	*serverHandle = INVALID_HANDLE;
}

typedef struct {
	u32 type;
	u64 cmdId;
	void* data;
	u32 dataSize;
} Request;

static Request ParseRequestFromTLS()
{
	Request req = {0};

	void* base = armGetTls();
	HipcParsedRequest hipc = hipcParseRequest(base);

	req.type = hipc.meta.type;

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

		req.cmdId = header->command_id;
		req.dataSize = dataSize - sizeof(CmifInHeader);
		req.data = req.dataSize ? ((u8*)header) + sizeof(CmifInHeader) : NULL;
	}

	return req;
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

// Currently not used
//static bool ReadPayload(const Request* req, void* data, u32 len)
//{
//	if (req->dataSize < len || !req->data)
//		return false;
//
//	memcpy(data, req->data, len);
//	return true;
//}

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

static u32 modeToSet = TYPE_MODE_INVALID;
static void ApplyModeChanges()
{
	if (modeToSet == TYPE_MODE_INVALID)
		return;

	SetModeID(modeToSet);

	modeToSet = TYPE_MODE_INVALID;
}

static bool HandleCommand(const Request* req)
{
	switch (req->cmdId)
	{
		case CMD_GET_VER:
		{
			u32 ver = SYSDVR_IPC_VERSION;
			WritePayloadResponseToTLS(0, &ver, sizeof(ver));
			return false;
		}
		case CMD_GET_MODE:
		{
			u32 mode = GetCurrentMode();
			WritePayloadResponseToTLS(0, &mode, sizeof(mode));
			return false;
		}
		case CMD_DEBUG_CRASH:
		{
			// Crash the process
			*(u32*)0x0 = 0xDEAD;
			return false;
		}
		case CMD_RESET_DISPLAY:
		{
			UtilSetConsoleScreenMode(true);
			return false;
		}
		case CMD_SET_USB:
		case CMD_SET_TCP:
		case CMD_SET_RTSP:
		case CMD_SET_OFF:
			// This relies nn the following conditions, otherwise it needs custom conversion code
			_Static_assert(CMD_SET_USB == TYPE_MODE_USB, "");
			_Static_assert(CMD_SET_TCP == TYPE_MODE_TCP, "");
			_Static_assert(CMD_SET_RTSP == TYPE_MODE_RTSP, "");
			_Static_assert(CMD_SET_OFF == TYPE_MODE_NULL, "");
			modeToSet = req->cmdId;
			WriteResponseToTLS(0);
			return false;
		default:
			WriteResponseToTLS(ERR_IPC_UNKCMD);
			return true;
	}
}

static bool IsClientConnected() {
	return *clientHandle != INVALID_HANDLE;
}

typedef enum { 
	// Command handled, no error
	IPC_OK,
	// Non fatal error
	IPC_AGAIN,
	// Request to terminate IPC server
	IPC_TERMINATE
} IpcStatus;

static IpcStatus WaitAndProcessRequest()
{
	s32 index = -1;
	Result rc = svcWaitSynchronization(&index, handles, IsClientConnected() ? 2 : 1, UINT64_MAX);

	// Handle common errors
	if (R_FAILED(rc))
	{
		LOG("svcWaitSynchronization: %x\n", rc);

		// Note: we currently don't use cancellation, but it's best to avoid throwing fatal here in case the OS or some other process tries to mess with us
		if (rc == KERNELRESULT(ThreadTerminating) || rc == KERNELRESULT(Cancelled))
			return IPC_TERMINATE;
		else if (rc == KERNELRESULT(InvalidHandle))
		{
			// The client might have disconnected unexpectedly
			if (IsClientConnected()) 
			{
				LOG("Unexpected client disconnection %x\n", rc);
				DisconnectClient();
				return IPC_AGAIN;
			}
			else
			{
				// Our server handle went bad, terminate the server. Let's avoid crashing the console.
				LOG("The IPC server is terminating due to %x\n", rc);
				return IPC_TERMINATE;
			}
		}

		// Other errors are fatal
		fatalThrow(rc);
	}

	if (index == 0)
	{
		Handle newcli;
		// Accept session
		if (R_FAILED(svcAcceptSession(&newcli, *serverHandle)))
			return IPC_AGAIN;
		
		// Max clients reached
		if (IsClientConnected())
		{
			svcCloseHandle(newcli);
			return IPC_AGAIN;
		}

		// New client connected, reset any pending state
		modeToSet = TYPE_MODE_INVALID;
		*clientHandle = newcli;
		return IPC_OK;
	}
	else if (index == 1)
	{
		if (!IsClientConnected()) {
			// This should never happen
			LOG("Received message but no client connected!\n");
			return IPC_AGAIN;
		}

		// Reeive the request
		s32 _idx;
		rc = svcReplyAndReceive(&_idx, clientHandle, 1, 0, UINT64_MAX);
		if (R_FAILED(rc))
		{
			LOG("svcReplyAndReceive (1): %x\n", rc);
			DisconnectClient();
			return IPC_AGAIN;
		}

		// Decode and handle the message
		bool shouldClose = false;
		Request r = ParseRequestFromTLS();
		switch (r.type)
		{
		case CmifCommandType_Request:
			shouldClose = HandleCommand(&r);
			break;
		case CmifCommandType_Close:
			WriteResponseToTLS(0);
			shouldClose = true;
			break;
		default:
			WriteResponseToTLS(ERR_HIPC_UNKREQ);
			break;
		}

		// Finally, send the response.
		rc = svcReplyAndReceive(&_idx, NULL, 0, *clientHandle, 0);
		if (R_FAILED(rc) && rc != KERNELRESULT(TimedOut))
		{
			LOG("svcReplyAndReceive (2): %x\n", rc);
			DisconnectClient();
			return IPC_AGAIN;
		}

		if (shouldClose) 
		{
			DisconnectClient();
			// If this is a clean disconnection, apply any pending mode changes now
			ApplyModeChanges();
		}

		return IPC_OK;
	}

	LOG("Unknown index from svcWaitSynchronization: %d\n", index);
	return IPC_AGAIN;
}

void IpcThread()
{
	LOG("IPC server starting\n");
	StartServer();
	LOG("IPC server started\n");

	while (1)
	{
		IpcStatus status = WaitAndProcessRequest();
		if (status == IPC_TERMINATE)
			break;
	}

	LOG("IPC server terminating\n");
	StopServer();
	LOG("IPC server terminated\n");
}
#endif