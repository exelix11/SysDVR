#pragma once
#include <stdint.h>
#include <stdbool.h>
#include "defines.h"

// This header defines the sysdvr protocol used for the initial handshake over USB and TCPBridge
// The handshake happens seprately for both streams

// The client sends a ProtohandshakeRequest to the endpoint, sysdvr replies with a single u32 result code.
// If the result code is Handshake_OK, the client can start streaming. Otherwise the connection is closed.

#define PROTO_REQUEST_MAGIC 0xAAAAAAAA

#define PROTO_HANDSHAKE_HELLO "SysDVR|" SYSDVR_PROTOCOL_VERSION

// This is sent by the console as soon as a client connects to communicate the protocol version.
// The string is null terminated.
_Static_assert(sizeof(PROTO_HANDSHAKE_HELLO) == 10, "Wrong size for the protocol hello message");

typedef enum
{
	Handshake_UnknownFailure = 0,
	Handshake_WrongVersion = 1,
	Handshake_InvalidArg = 2,
	Handshake_InvalidSize = 3,
	Handshake_InvalidMeta = 4,
	Handshake_WrongMagic = 5,
	Handshake_Ok = 6,
	Hanshake_InvalidChannel = 7,
} ProtoHandshakeResult;

typedef struct {
	ProtoHandshakeResult Result;
	bool RequestedAudio;
	bool RequestedVideo;
} ProtoParsedHandshake;

enum ProtoMetaFlags // 8 bits
{
	ProtoMeta_Video = (1 << 0),
	ProtoMeta_Audio = (1 << 1),
};

enum ProtoMetaVideoFlags // 8 bits
{
	ProtoMetaVideo_UseNalHash = (1 << 0),
	ProtoMetaVideo_InjectPPSSPS = (1 << 1),
	ProtoMetaVideo_NalHashOnlyIDR = (1 << 2),
};

enum ExtraFeatureFlags // 8 bits
{
	ExtraFeatureFlags_None = 0,
	// Request the console to turn off the display during streaming
	ExtraFeatureFlags_TurnOffScreen = 1,
};

// Sent by the client to set up streaming
struct ProtoHandshakeRequest
{
	uint32_t Magic;
	// Two ascii chars from SYSDVR_PROTOCOL_VERSION
	uint16_t ProtoVer;

	uint8_t MetaFlags; // ProtoMetaFlags

	uint8_t VideoFlags; // ProtoMetaVideoFlags
	uint8_t AudioBatching;

	uint8_t FeatureFlags;
	uint8_t Reserved[6];
};

_Static_assert(sizeof(struct ProtoHandshakeRequest) == 16);

#define PROTO_HANDSHAKE_SIZE sizeof(struct ProtoHandshakeRequest)

typedef enum {
	ProtoHandshakeAccept_Any,
	ProtoHandshakeAccept_Video,
	ProtoHandshakeAccept_Audio,
} ProtoHandshakeAccept;

// Verifies the handshare and if valid sets the global state of the client
ProtoParsedHandshake ProtoHandshake(ProtoHandshakeAccept config, uint8_t* data, int length);

// Must be called every time ProtoHandshare returned OK, it is safe to call multiple times
void ProtoClientGlobalStateDisconnected();
