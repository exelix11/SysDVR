#include <string.h>
#include "proto.h"
#include "../capture.h"
#include "defines.h"
#include "../core.h"
#include "../util.h"

static inline bool hasVideo(const struct ProtoHandshakeRequest* req)
{
	return req->MetaFlags & ProtoMeta_Video;
}

static inline bool hasAudio(const struct ProtoHandshakeRequest* req)
{
	return req->MetaFlags & ProtoMeta_Audio;
}

static ProtoHandshakeResultCode ProtoHandshakeVersion(uint8_t* data, int length, struct ProtoHandshakeRequest* out_req)
{
	if (length != sizeof(struct ProtoHandshakeRequest))
		return Handshake_InvalidSize;

	memcpy(out_req, data, sizeof(*out_req));

	if (out_req->Magic != PROTO_REQUEST_MAGIC)
		return Handshake_WrongMagic;

	if (memcmp(&out_req->ProtoVer, SYSDVR_PROTOCOL_VERSION, 2))
		return Handshake_WrongVersion;

	bool video = hasVideo(out_req);
	bool audio = hasAudio(out_req);

	if (!video && !audio)
		return Handshake_InvalidMeta;

	return Handshake_Ok;
}

static bool screenStateModified = false;

static void QueryMemoryState(ProtoParsedHandshake* response)
{
	u64 application_size = 0, application_used = 0;
	u64 applet_size = 0, applet_used = 0;
	u64 system_size = 0, system_used = 0;
	u64 system_unsafe_size = 0, system_unsafe_used = 0;

	Result rc = svcGetSystemInfo(&application_size, SystemInfoType_TotalPhysicalMemorySize, INVALID_HANDLE, PhysicalMemorySystemInfo_Application);
	if (R_SUCCEEDED(rc)) rc = svcGetSystemInfo(&application_used, SystemInfoType_UsedPhysicalMemorySize, INVALID_HANDLE, PhysicalMemorySystemInfo_Application);
	if (R_SUCCEEDED(rc)) rc = svcGetSystemInfo(&applet_size, SystemInfoType_TotalPhysicalMemorySize, INVALID_HANDLE, PhysicalMemorySystemInfo_Applet);
	if (R_SUCCEEDED(rc)) rc = svcGetSystemInfo(&applet_used, SystemInfoType_UsedPhysicalMemorySize, INVALID_HANDLE, PhysicalMemorySystemInfo_Applet);
	if (R_SUCCEEDED(rc)) rc = svcGetSystemInfo(&system_size, SystemInfoType_TotalPhysicalMemorySize, INVALID_HANDLE, PhysicalMemorySystemInfo_System);
	if (R_SUCCEEDED(rc)) rc = svcGetSystemInfo(&system_used, SystemInfoType_UsedPhysicalMemorySize, INVALID_HANDLE, PhysicalMemorySystemInfo_System);
	if (R_SUCCEEDED(rc)) rc = svcGetSystemInfo(&system_unsafe_size, SystemInfoType_TotalPhysicalMemorySize, INVALID_HANDLE, PhysicalMemorySystemInfo_SystemUnsafe);
	if (R_SUCCEEDED(rc)) rc = svcGetSystemInfo(&system_unsafe_used, SystemInfoType_UsedPhysicalMemorySize, INVALID_HANDLE, PhysicalMemorySystemInfo_SystemUnsafe);

	response->Result.MemoryPools.QueryResult = rc;
	response->Result.MemoryPools.ApplicationSize = application_size;
	response->Result.MemoryPools.ApplicationUsed = application_used;
	response->Result.MemoryPools.AppletSize = applet_size;
	response->Result.MemoryPools.AppletUsed = applet_used;
	response->Result.MemoryPools.SystemSize = system_size;
	response->Result.MemoryPools.SystemUsed = system_used;
	response->Result.MemoryPools.SystemUnsafeSize = system_unsafe_size;
	response->Result.MemoryPools.SystemUnsafeUsed = system_unsafe_used;
}

static void ProtoGlobalClientStateConnect(uint8_t protocolFlags, ProtoParsedHandshake* response)
{
	if (protocolFlags & ExtraFeatureFlags_TurnOffScreen)
	{
		UtilSetConsoleScreenMode(false);
		screenStateModified = true;
	}

	if (protocolFlags & ExtraFeatureFlags_MemoryDiag)
	{
		QueryMemoryState(response);
	}
	else
	{
		response->Result.MemoryPools.QueryResult = UINT32_MAX;
	}
}

void ProtoClientGlobalStateDisconnected()
{
	if (screenStateModified)
	{
		screenStateModified = false;
		UtilSetConsoleScreenMode(true);
	}
}

ProtoParsedHandshake ProtoHandshake(ProtoHandshakeAccept config, uint8_t* data, int length)
{
	struct ProtoHandshakeRequest req;
	
	ProtoParsedHandshake res = {};
	res.Result.Code = ProtoHandshakeVersion(data, length, &req);

	// The caller may only accept video or audio
	if (res.Result.Code == Handshake_Ok)
	{
		if (config == ProtoHandshakeAccept_Video && hasAudio(&req))
			res.Result.Code = Hanshake_InvalidChannel;
		else if (config == ProtoHandshakeAccept_Audio && hasVideo(&req))
			res.Result.Code = Hanshake_InvalidChannel;
	}

	if (res.Result.Code != Handshake_Ok)
	{
		LOG("Handshake failed with code %d\n", res.Result);
		return res;
	}

	if (req.MetaFlags & ProtoMeta_Video) 
	{
		CaptureSetNalHashing(req.VideoFlags & ProtoMetaVideo_UseNalHash, req.VideoFlags & ProtoMetaVideo_NalHashOnlyIDR);
		CaptureSetPPSSPSInject(req.VideoFlags & ProtoMetaVideo_InjectPPSSPS);

		res.RequestedVideo = true;
	}

	if (req.MetaFlags & ProtoMeta_Audio)
	{
		if (CaptureSetAudioBatching(req.AudioBatching) != req.AudioBatching)
			res.Result.Code = Handshake_InvalidArg;

		res.RequestedAudio = true;
	}

	if (res.Result.Code == Handshake_Ok)
	{
		ProtoGlobalClientStateConnect(req.FeatureFlags, &res);
	}

	return res;
}
