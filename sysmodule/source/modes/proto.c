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

static inline bool hasAusio(const struct ProtoHandshakeRequest* req)
{
	return req->MetaFlags & ProtoMeta_Audio;
}

static ProtoHandshakeResult ProtoHandshakeVersion(uint8_t* data, int length, struct ProtoHandshakeRequest* out_req)
{
	if (length != sizeof(struct ProtoHandshakeRequest))
		return Handshake_InvalidSize;

	memcpy(out_req, data, sizeof(*out_req));

	if (out_req->Magic != PROTO_REQUEST_MAGIC)
		return Handshake_WrongMagic;

	if (memcmp(&out_req->ProtoVer, SYSDVR_PROTOCOL_VERSION, 2))
		return Handshake_WrongVersion;

	bool video = hasVideo(out_req);
	bool audio = hasAusio(out_req);

	if (!video && !audio)
		return Handshake_InvalidMeta;

	return Handshake_Ok;
}

static bool screenStateModified = false;

static void ProtoGlobalClientStateConnect(uint8_t protocolFlags)
{
	if (protocolFlags & ExtraFeatureFlags_TurnOffScreen)
	{
		UtilSetConsoleScreenMode(false);
		screenStateModified = true;
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
	res.Result = ProtoHandshakeVersion(data, length, &req);

	// The caller may only accept video or audio
	if (res.Result == Handshake_Ok) 
	{
		if (config == ProtoHandshakeAccept_Video && hasAusio(&req))
			res.Result = Hanshake_InvalidChannel;
		else if (config == ProtoHandshakeAccept_Audio && hasVideo(&req))
			res.Result = Hanshake_InvalidChannel;
	}

	if (res.Result != Handshake_Ok) 
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
			res.Result = Handshake_InvalidArg;

		res.RequestedAudio = true;
	}

	if (res.Result == Handshake_Ok) 
	{
		ProtoGlobalClientStateConnect(req.FeatureFlags);
	}

	return res;
}