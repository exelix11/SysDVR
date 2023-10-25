#pragma once
#include <stdint.h>

// This header defines the sysdvr protocol used for the initial handshake over USB and TCPBridge
// The handshake happens seprately for both streams

// The client sends a ProtohandshakeRequest to the endpoint, sysdvr replies with a single u32 result code.
// If the result code is Handshake_OK, the client can start streaming. Otherwise the connection is closed.

#define PROTO_REQUEST_MAGIC 0xAAAAAAAA

typedef enum
{
	Handshake_UnknownFailure = 0,
	Handshake_WrongVersion = 1,
	Handshake_InvalidArg = 2,
	Handshake_InvalidSize = 3,
	Handshake_InvalidMeta = 4,
	Handshake_WrongMagic = 5,
	Handshake_Ok = 6,
} ProtoHandshakeResult;

typedef struct {
	ProtoHandshakeResult Result;
	bool RequestedAudio;
	bool RequestedVideo;
} ProtoParsedHandshake;

// Sent by the client to set up streaming
struct ProtoHandshakeRequest
{
	u32 Magic;
	// Two ascii chars from SYSDVR_PROTOCOL_VERSION
	u16 ProtoVer;

	struct {
		u8 Audio : 1;
		u8 Video : 1;
		u8 Reserved : 6;
	} Meta;

	struct {
		u8 Batching;
	} Audio;

	struct {
		u8 UseNalHash : 1;
		u8 InjectPPSSPS : 1;
		u8 Reserved : 6;
	} Video;

	u8 Reserved[7];
};

_Static_assert(sizeof(struct ProtoHandshakeRequest) == 16);

#define PROTO_HANDSHAKE_SIZE sizeof(struct ProtoHandshakeRequest)

ProtoParsedHandshake ProtoHandshake(uint8_t* data, int length);

