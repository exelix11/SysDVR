#!/bin/sh

set -e

echo Checking dependencies...

# Ensure SDL sources are present
if [ ! -d "app/jni/SDL/src" ]; then
	echo Downloading SDL sources...
	wget https://github.com/libsdl-org/SDL/releases/download/release-2.28.1/SDL2-2.28.1.tar.gz
	tar xf SDL2-2.28.1.tar.gz
	mv $(pwd)/SDL2-2.28.1/src ./app/jni/SDL/
	mv $(pwd)/SDL2-2.28.1/include ./app/jni/SDL/
	rm -rf SDL2-2.28.1
	rm SDL2-2.28.1.tar.gz
fi

# Ensure SDL_image sources are present
if [ ! -d "app/jni/SDL_Image" ]; then
	echo Downloading SDL_image sources...
	wget https://github.com/libsdl-org/SDL_image/releases/download/release-2.6.3/SDL2_image-2.6.3.tar.gz
	tar xf SDL2_image-2.6.3.tar.gz
	mv $(pwd)/SDL2_image-2.6.3 ./app/jni/SDL_Image
	rm -rf SDL2_image-2.0.5
	rm SDL2_image-2.6.3.tar.gz
fi

# Ensure cimgui sources are present
if [ ! -d "app/jni/cimgui/cimgui" ]; then
	echo Downloading cimgui sources...
	wget https://github.com/exelix11/CimguiSDL2Cross/archive/refs/heads/master.zip
	unzip master.zip > /dev/null
	mv $(pwd)/CimguiSDL2Cross-master/cimgui ./app/jni/cimgui/
	mv $(pwd)/CimguiSDL2Cross-master/Android.mk ./app/jni/cimgui/
	rm -rf $(pwd)/CimguiSDL2Cross-master/
	rm master.zip
fi

if [ ! -d "app/libs/" ]; then
	mkdir app/libs/
fi

# Ensure android ffmpeg binaries are present
if [ ! -e "app/libs/ffmpeg-kit-full-5.1.LTS_trimmed.aar" ]; then
	echo Downloading android ffmpeg-kit builds...
	wget https://github.com/arthenica/ffmpeg-kit/releases/download/v5.1.LTS/ffmpeg-kit-full-5.1.LTS.aar
	# We manually trim ffmpegkit to get rid of architectures we don't need
	# This reduces the final APK size from 120MB to 60MB
	echo Trimming ffmpeg-kit
	unzip -q ffmpeg-kit-full-5.1.LTS.aar -d ffmpegkit
	rm -rf ffmpegkit/armeabi-v7a ffmpegkit/x86_64 ffmpegkit/x86 ffmpegkit/jni/x86_64 ffmpegkit/jni/x86 ffmpegkit/jni/armeabi-v7a
	7z a ffmpegkit.aar.zip ./ffmpegkit/*
	mv ffmpegkit.aar.zip app/libs/ffmpeg-kit-full-5.1.LTS_trimmed.aar
	rm -rf ffmpegkit
	rm ffmpeg-kit-full-5.1.LTS.aar
fi

BFLAT=$(pwd)/bflat/bflat

# Check if bflat is available
if [ ! -e $BFLAT ]; then
	echo Downloading bflat...
	wget https://github.com/bflattened/bflat/releases/download/v7.0.2/bflat-7.0.2-linux-glibc-x64.tar.gz
	mkdir bflat
	tar xf bflat-7.0.2-linux-glibc-x64.tar.gz --strip-components=1 -C bflat
	rm bflat-7.0.2-linux-glibc-x64.tar.gz
fi

# Move to client root
cd ../../

# Seems to be needed to avoid conflicts with the dotnet build system
rm -rf obj/

echo Building client...
$BFLAT build --os:linux --arch:arm64 --libc:bionic --no-reflection --no-globalization -d ANDROID_LIB --ldflags "-soname libClient.so" -r Platform/Android/FFmpeg.AutoGen.dll -r bin/Debug/net7.0/LibUsbDotNet.dll

mv libClient.so Platform/Android/app/jni/Client/libClient.so
cd Platform/Android/