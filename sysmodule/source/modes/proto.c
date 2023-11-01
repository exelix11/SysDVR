#include <string.h>
#include "proto.h"
#include "../capture.h"
#include "defines.h"
#include "../core.h"

static ProtoHandshakeResult ProtoHandshakeVersion(uint8_t* data, int length, struct ProtoHandshakeRequest* out_req)
{
	if (length != sizeof(struct ProtoHandshakeRequest))
		return Handshake_InvalidSize;

	memcpy(out_req, data, sizeof(*out_req));

	if (out_req->Magic != PROTO_REQUEST_MAGIC)
		return Handshake_WrongMagic;

	if (memcmp(&out_req->ProtoVer, SYSDVR_PROTOCOL_VERSION, 2))
		return Handshake_WrongVersion;

	bool video = out_req->MetaFlags & ProtoMeta_Video;
	bool audio = out_req->MetaFlags & ProtoMeta_Audio;

	if (!video && !audio)
		return Handshake_InvalidMeta;

	return Handshake_Ok;
}

ProtoParsedHandshake ProtoHandshake(uint8_t* data, int length)
{
	struct ProtoHandshakeRequest req;
	ProtoHandshakeResult rc = ProtoHandshakeVersion(data, length, &req);
	
	ProtoParsedHandshake res = {
		.Result = rc
	};

	if (rc != Handshake_Ok) {
		LOG("Handshake failed with code %d\n", rc);
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

	return res;
}