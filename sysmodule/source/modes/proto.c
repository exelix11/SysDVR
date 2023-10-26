#include <string.h>
#include "proto.h"
#include "../capture.h"
#include "defines.h"

static ProtoHandshakeResult ProtoHandshakeVersion(uint8_t* data, int length, struct ProtoHandshakeRequest* out_req)
{
	if (length != sizeof(struct ProtoHandshakeRequest))
		return Handshake_InvalidSize;

	memcpy(out_req, data, sizeof(*out_req));

	if (out_req->Magic != PROTO_REQUEST_MAGIC)
		return Handshake_WrongMagic;

	if (memcmp(&out_req->ProtoVer, SYSDVR_PROTOCOL_VERSION, 2))
		return Handshake_WrongVersion;

	if (!out_req->Meta.Video && !out_req->Meta.Audio)
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

	if (rc != Handshake_Ok)
		return res;

	if (req.Meta.Video) 
	{
		CaptureSetNalHashing(req.Video.UseNalHash);
		CaptureSetPPSSPSInject(req.Video.InjectPPSSPS);

		res.RequestedVideo = true;
	}

	if (req.Meta.Audio)
	{
		if (CaptureSetAudioBatching(req.Audio.Batching) != req.Audio.Batching)
			res.Result = Handshake_InvalidArg;

		res.RequestedAudio = true;
	}

	return res;
}