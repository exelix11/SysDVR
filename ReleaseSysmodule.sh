#!/bin/bash
set -e

echo building for release
rm -rf SysmoduleRelease
mkdir -p SysmoduleRelease/Debug/

cd SysDVRConfig
make clean
make -j4

cd ../sysmodule
make clean
make -j4 DEFINES="-DRELEASE"
mkdir -p ../SysmoduleRelease/Main/atmosphere/contents/00FF0000A53BB665/flags 
mkdir -p ../SysmoduleRelease/Main/switch 
mkdir -p ../SysmoduleRelease/Main/config/sysdvr 
echo . > ../SysmoduleRelease/Main/config/sysdvr/rtsp
cp sysmodule.nsp ../SysmoduleRelease/Main/atmosphere/contents/00FF0000A53BB665/exefs.nsp
cp sysmodule.elf ../SysmoduleRelease/Debug/main.elf
cp toolbox.json ../SysmoduleRelease/Main/atmosphere/contents/00FF0000A53BB665/
echo . > ../SysmoduleRelease/Main/atmosphere/contents/00FF0000A53BB665/flags/boot2.flag
cp ../SysDVRConfig/SysDVR-conf.nro ../SysmoduleRelease/Main/switch/SysDVR-conf.nro
7z a ../SysmoduleRelease/SysDVR.zip ../SysmoduleRelease/Main/*

make clean
make -j4 DEFINES="-DRELEASE -DUSB_ONLY" 
mkdir -p ../SysmoduleRelease/USB/atmosphere/contents/00FF0000A53BB665/flags 
cp sysmodule.nsp ../SysmoduleRelease/USB/atmosphere/contents/00FF0000A53BB665/exefs.nsp
cp sysmodule.elf ../SysmoduleRelease/Debug/usbOnly.elf
cp toolbox.usb.json ../SysmoduleRelease/USB/atmosphere/contents/00FF0000A53BB665/toolbox.json
echo . > ../SysmoduleRelease/USB/atmosphere/contents/00FF0000A53BB665/flags/boot2.flag
7z a ../SysmoduleRelease/USB.zip ../SysmoduleRelease/USB/*

make clean
echo done.