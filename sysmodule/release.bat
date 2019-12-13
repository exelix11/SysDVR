@echo off
set TITLEID=00FF0000A53BB665

set PATH=C:\Program Files\7-Zip\;%PATH%

echo building for release
rmdir /S /Q Release

cd ..\SysDVRConfig
make clean
make || goto quit

cd ..\sysmodule
make clean
make DEFINES="-DRELEASE" || goto quit
mkdir Release\Main\atmosphere\contents\%TITLEID% > nul
mkdir Release\Main\atmosphere\contents\%TITLEID%\flags > nul
mkdir Release\Main\switch > nul
mkdir Release\Main\config\sysdvr > nul
echo . > Release\Main\config\sysdvr\tcp
copy sysmodule.nsp Release\Main\atmosphere\contents\%TITLEID%\exefs.nsp
echo . > Release\Main\atmosphere\contents\%TITLEID%\flags\boot2.flag
copy ..\SysDVRConfig\SysDVR-conf.nro Release\Main\switch\SysDVR-conf.nro
7z a Release\SysDVR.zip .\Release\Main\*

make clean
make DEFINES="-DRELEASE -DUSB_ONLY" || goto quit
mkdir Release\USB\atmosphere\contents\%TITLEID% > nul
mkdir Release\USB\atmosphere\contents\%TITLEID%\flags > nul
copy sysmodule.nsp Release\USB\atmosphere\contents\%TITLEID%\exefs.nsp
echo . > Release\USB\atmosphere\contents\%TITLEID%\flags\boot2.flag
7z a Release\USB.zip .\Release\USB\*

make clean
echo done.
:quit
pause