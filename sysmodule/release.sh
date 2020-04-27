#!/bin/bash
set -e

echo building for release
rm -rf Release
mkdir -p Release/Debug/

cd ../SysDVRConfig
make clean
make

cd ../sysmodule
make clean
make -j4 DEFINES="-DRELEASE"
mkdir -p Release/Main/atmosphere/contents/00FF0000A53BB665/flags 
mkdir -p Release/Main/switch 
mkdir -p Release/Main/config/sysdvr 
echo . > Release/Main/config/sysdvr/rtsp
cp sysmodule.nsp Release/Main/atmosphere/contents/00FF0000A53BB665/exefs.nsp
cp sysmodule.elf Release/Debug/main.elf
echo . > Release/Main/atmosphere/contents/00FF0000A53BB665/flags/boot2.flag
cp ../SysDVRConfig/SysDVR-conf.nro Release/Main/switch/SysDVR-conf.nro
7z a Release/SysDVR.zip ./Release/Main/*

make clean
make -j4 DEFINES="-DRELEASE -DUSB_ONLY" 
mkdir -p Release/USB/atmosphere/contents/00FF0000A53BB665/flags 
cp sysmodule.nsp Release/USB/atmosphere/contents/00FF0000A53BB665/exefs.nsp
cp sysmodule.elf Release/Debug/usbOnly.elf
echo . > Release/USB/atmosphere/contents/00FF0000A53BB665/flags/boot2.flag
7z a Release/USB.zip ./Release/USB/*

make clean
echo done.