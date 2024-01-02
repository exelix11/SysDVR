# The SysDVR protocol

This file documents the SysDVR protocol that is used for communication between the sysmoudle and the client when streaming over network "tcp bridge" or usb.
This protocol is not used when the sysmodule is set to "simple network" mode where it just uses RTSP and you just need a normal video player.

This reference is valid as of 6.0. Consider comparing what is written here with the code since i might forget to update this if the protocol changes.

SysDVR can stream over different channels, the protocol has been designed to accomodate for various limitations of the different channels. Each channel has its own discovery and connection mechanism, after that the actual streaming protocol is the same.

# USB Streaming

SysDVR presents itself as a device with VID `18D1` and PID `4EE0`, this is the pair of values used by the `Nexus/Pixel Device (fastboot)`, to detect a real SysDVR device you must check that the serial string of the device matches is a valid _string descriptor_ (explained later).

To communicate you open a handle to the device, the device has a single interface (this was not true before 6.0) with a single pair of endpoints, one for writing and one for reading. All data is exchanged through bulk transfers.

Audio and video packets are multiplexed through the same stream provided by these two endpoints.

The rest of the communication is described in the `Protocol` part of this page.

## Usb shenanigans

As we will see the protocol is composed of transfers of a fixed header followed by a number of bytes defined in the header, for USB this happens in a single bulk transfer.

An average implementation may do the following:
```
header = read(sizeof(header))
data = read(header.size)
```

However note that not all USB implementations support working as data streams, for example Windows will buffer the content of bulk transfers and allow a consumer to read it over multiple receive calls as the previous example.

Libusb does not seem to support this and you will instead need to prepare a big backing buffer to read the whole packet and then parse it manually. To allow this, the protocol defines a max transfer size of `max_data_buffer + size_of_packet_header`, a proper calculation is shown in the `Data transfer` section of this page. Note that this max size may change in different protocol versions.

For maximum compatibility SysDVR client uses the fixed size approach.

Reference implementation: https://github.com/exelix11/SysDVR/blob/6.0/Client/Sources/UsbStreaming.cs#L90

## Windows USB driver shenanigans

SysDVR client on Windows uses Microsoft's own WinUSB driver, however to make the association between a device ID and the driver itself we need an inf descriptor file that is signed by a trusted party. 

This is why we reuse a well-known device ID so that we can install google's signed Android USB drivers on Windows making the configuration painless.

Source for the driver management code: https://github.com/exelix11/SysDVR/blob/6.0/Client/Platform/Specific.Win/WinDriverInstall.cs

The direct link to the current driver is https://dl.google.com/android/repository/usb_driver_r13-windows.zip

# TCP Bridge

TCP Bridge works on two different sockets, one for each stream. This is because when network streaming usually the bottleneck is the socket buffers.

The client connects to port 9911 for video and port 9922 for audio. In case of single channel streaming you can connect to only one of the ports. Each connection is independent, the initial handshake described in `Protocol` is repeated for each socket. 

In case the handshake fails for one of the two connections the other one can continue streaming just fine, this however should never happen normally.

## Network discovery

Every 2 seconds the sysmodule broadcasts a UDP packet on port 19999, this contains a _string descriptor_ (explained later). A client can detect the sysmodule by listening for this packet.

Reference implementation: https://github.com/exelix11/SysDVR/blob/6.0/Client/Sources/NetworkScan.cs

# Protocol

The streaming protocol uses a custom packetization format, this is the same across USB and network, as previously explained the connection phase is different for each transport medium.

Data endianness is little endian, all data structures are defined without implicit compiler packing.

## Device descriptor

This is a string sent by the sysmodule that encodes metadata such as the protocol version and the console serial.

This is used during the discovery phase to search for devices, for USB it is the serial of the device while for network mode it is the discovery packet that the console broadcasts while waiting for a connection.

This is a plaintext string with no binary data so it fits into the USB serial field.

Implementing the device descriptor is not needed as this is used as a pre-connection validation of the protocol version, the protocol is negotiated again during the actual connection phase.

Reference decoder source: https://github.com/exelix11/SysDVR/blob/6.0/Client/Core/StreamInfo.cs#L49
Reference encoder source: https://github.com/exelix11/SysDVR/blob/6.0/sysmodule/source/core.c#L30

The string looks like the following: `SysDVR|6.0|00|SERIAL NUMBER`, fields are separated by the `|` character. The length of the string is not fixed and the termination depends on the source. You should terminate the string at the first ASCII nul byte however this might not be present in all transport mediums (for example, USB will provide an exact length, UDP packets may be padded with NULL bytes).

The string is composed by the following fields:
- `SysDVR`: fixed "magic value" of the string.
- `6.0` : string version of the client, there is no character limit on this field. This is only for displaying and you should not rely on this value to detect compatibility.
- `00` : Protocol version. This must be exactly two ascii characters. The meaning of the values is not defined, the client should only accept values that it knows as valid. Currently `00` is the only valid value.
- `SERIAL NUMBER` : This is the serial number of the console, can be a string of any length, the sysmodule sets it to a string of sizeof(SetSysSerialNumber) due to libnx implementation. 

## Handshake

After the client is connected with the console it must perform an initial handshake, it sends a data structure that configures the parameters of the stream.

handshake packet reference: https://github.com/exelix11/SysDVR/blob/6.0/sysmodule/source/modes/proto.h

The client should set the bitfield `MetaFlags` of the request packet to the stream it wants to subscribe to, that is `ProtoMeta_Video` or `ProtoMeta_Audio` or both. Note that for TCP bridge you should only set the bit of the stream of the socket you are connected to or the handshake will fail.

The meaning of the configurable parameters is explained below:
- `UseNalHash` : Allows replay of packets using the hash-slot property of the protocol. The client stores the last _implementation defined_ number of packets so that the sysmodule can request to replay them without having to send them. This solves glitching in specific conditions.
- `NalHashOnlyIDR`: This applies the previous optimization only on keyframes and not the whole stream
- `InjectPPSSPS`: This requests the sysmodule to manually inject in the stream copies of the hardcoded PPS and SPS (h264-specific metadata packets) in the stream. These are needed by video players and if they are not received at least once the stream can't be decoded. Note that the official client also has an hardcoded copy of the expected parameters to initialize the player beforehand. If you manipulate the encoder settings by tweaking system options or patching grc you may have to capture the new parameters and update the sysmodule and client, the console will only emit SPS and PPS once when a game is launched.
- `AudioBatching`: A value from 0 to 3 (inclusive) of how many packets of audio should the sysmodule buffer before sending them.

Video properties are only applied if `MetaFlags` contains `Video`, likewise audio properties are only applied if it contains `Audio`. When using TCP bridge it is safe to use the same handshake packet with the only difference being the `MetaFlags` field.

client implementation reference: https://github.com/exelix11/SysDVR/blob/6.0/Client/Sources/Protocol.cs#L165

The sysmodule replies with a single uint32 that represents the result code as defined in `ProtoHandshakeResult`. If the result is not `Handshake_Ok` the sysmodule will also close the connection.

## Data transfer 

If the handshake was succesful, the sysmodule continuously sends data to the client with no acknowledgement needed.
The packet format is defined in https://github.com/exelix11/SysDVR/blob/6.0/sysmodule/source/capture.h#L42

The magic number in the header is composed of 4 repeating `CC` bytes, this can be used to resync the stream in case of a decoding error by simply skipping every byte until a sequence of `CCCCCCCC` is found.

When multiplexing over the same connection the contents of the packet can be detected from the `MetaData` field, it will have only one of the `PacketMeta_Type_Video` or `PacketMeta_Type_Audio` bits set.

The client should read `DataSize` bytes after the header to receive the full packet, except when the `PacketMeta_Content_Replay` bit is set. In that case the client should simulate a packet being received with the content indexed by the value of the `ReplaySlot` field. Replay packets currently only happen over the video stream and only when the `UseNalHash` option was requested during the handshake.

The protocol defines a max possible overall packet size that is `max(VbufSz, AbufSz * MaxABatching) + sizeof(PacketHeader)`, this means `DataSize` can be at most `max(VbufSz, AbufSz * MaxABatching)`. SysDVR will never try to send a packet bigger than this, however to detect potential communication errors you should validate the `DataSize` field before attempting to read the data.

Furthermore, when `UseNalHash` is enabled and the current packet is not a replay packet the client should store a copy of the content with the value of `ReplaySlot` as key, this will need to be recalled when a replay packet with the same key is received. The sysmodule has a limited number of slots (currently 20) and will reuse numbers as it sees fit, you should not implement any kind of garbage collection.

The content of the packet after the header is raw stream data, for video it is one or more H264 NALs and for audio 16bit pcm samples. You should feed these directly to your decoder.