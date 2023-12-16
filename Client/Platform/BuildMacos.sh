#!/bin/sh

set -e

git rev-parse --short HEAD > Resources/resources/buildid.txt

cd .. # Go to client root

echo Checking dependencies...

# All dependencies for macos are "fat" binaries meaning they contain both x64 and arm64 code
# so we can just download them once and use them for both architectures
# To avoid duplicate files we manually copy them in the release zip instead of puttig them in the runtimes folder
mkdir macos-deps
mkdir tmp

if [ ! -e "macos-deps/cimgui.dylib" ]; then
	echo Downloading Cimgui...
	curl -L https://github.com/exelix11/CimguiSDL2Cross/releases/download/r1/macos-fat-binaries.zip -o macos-fat-binaries.zip
	7z x macos-fat-binaries.zip -Omacos-deps
	rm macos-fat-binaries.zip
	rm -rf macos-deps/Resources
fi

if [ ! -e "macos-deps/SDL2_image.dylib" ]; then
	echo Downloading SDL2-image...
	curl -L https://github.com/libsdl-org/SDL_image/releases/download/release-2.6.3/SDL2_image-2.6.3.dmg -o SDL2_image.dmg
	dmg2img -i SDL2_image.dmg -o tmp.hfs
	7z x tmp.hfs -otmp
	rm SDL2_image.dmg tmp.hfs
	mv tmp/SDL2_image/SDL2_image.framework/Versions/A/SDL2_image macos-deps/SDL2_image.dylib
	rm -rf tmp/SDL2_image
fi

if [ ! -e "macos-deps/libavcodec.dylib" ]; then
	echo Downloading ffmpeg...
	curl -L https://github.com/arthenica/ffmpeg-kit/releases/download/v5.1/ffmpeg-kit-full-gpl-5.1-macos-xcframework.zip -o ffmpeg.zip
	7z x ffmpeg.zip -Otmp
	rm ffmpeg.zip

	mv tmp/libavcodec.xcframework/macos-arm64_x86_64/libavcodec.framework/Versions/A/libavcodec macos-deps/libavcodec.dylib
	mv tmp/libavdevice.xcframework/macos-arm64_x86_64/libavdevice.framework/Versions/A/libavdevice macos-deps/libavdevice.dylib
	mv tmp/libavfilter.xcframework/macos-arm64_x86_64/libavfilter.framework/Versions/A/libavfilter macos-deps/libavfilter.dylib
	mv tmp/libavformat.xcframework/macos-arm64_x86_64/libavformat.framework/Versions/A/libavformat macos-deps/libavformat.dylib
	mv tmp/libavutil.xcframework/macos-arm64_x86_64/libavutil.framework/Versions/A/libavutil macos-deps/libavutil.dylib
	mv tmp/libswresample.xcframework/macos-arm64_x86_64/libswresample.framework/Versions/A/libswresample macos-deps/libswresample.dylib	
	mv tmp/libswscale.xcframework/macos-arm64_x86_64/libswscale.framework/Versions/A/libswscale macos-deps/libswscale.dylib	

	rm -rf tmp/*
fi

if [ ! -e "macos-deps/libusb-1.0.0.dylib" ]; then
	echo Downloading libusb
	# Note the name difference 1.0.0 -> 1.0
	curl -L https://github.com/exelix11/libusb-builds/releases/download/v0/libusb-1.0.0.dylib -o macos-deps/libusb-1.0.dylib
fi

# Stip all dylibs signatures, this is a complicated mess to figure out and i don't have a mac
# so we're doing it the windows way(tm) with no signatures at all
codesign --remove-signature -v macos-deps/*.dylib

echo Building x64 client...
dotnet publish -c Release -r osx-x64 /p:SysDvrTarget=macos

mkdir MacOsBuild-x64
cp -r bin/Release/net8.0/osx-x64/publish/* MacOsBuild-x64/
mkdir -p MacOsBuild-x64/runtimes/osx-x64/native
cp macos-deps/* MacOsBuild-x64/runtimes/osx-x64/native/
cd MacOsBuild-x64
chmod +x SysDVR-Client
zip -r ../SysDVRClient-MacOs-x64.zip *
cd ..

echo Building arm64 client...
dotnet publish -c Release -r osx-arm64 /p:SysDvrTarget=macos

mkdir MacOsBuild-arm64
cp -r bin/Release/net8.0/osx-arm64/publish/* MacOsBuild-arm64/
mkdir -p MacOsBuild-arm64/runtimes/osx-arm64/native
cp macos-deps/* MacOsBuild-arm64/runtimes/osx-arm64/native/
cd MacOsBuild-arm64
chmod +x SysDVR-Client
zip -r ../SysDVRClient-MacOs-arm64.zip *
cd ..

echo Done !