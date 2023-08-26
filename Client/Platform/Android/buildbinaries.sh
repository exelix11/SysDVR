#!/bin/sh
cd ../../
bflat build --os:linux --arch:arm64 --libc:bionic --no-reflection --no-globalization -d ANDROID_LIB --ldflags "-soname libsysdvr.so" -r bin/Debug/net7.0/FFmpeg.AutoGen.dll -r bin/Debug/net7.0/LibUsbDotNet.dll -r bin/Debug/net7.0/RTSP.dll -r bin/Debug/net7.0/Microsoft.Extensions.Logging.Abstractions.dll
mv Client Platform/Android/app/jni/sysdvr/libsysdvr.so
cd Platform/Android/