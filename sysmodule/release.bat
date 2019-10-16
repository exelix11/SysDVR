@echo off
set TITLEID=00FF0000A53BB665

set PATH=C:\Program Files\7-Zip\;%PATH%

echo building for release
rmdir /S /Q Release

make clean
make DEFINES="-DRELEASE -DMODE_USB" || goto quit
mkdir Release\USB\atmosphere\titles\%TITLEID% > nul
mkdir Release\USB\atmosphere\titles\%TITLEID%\flags > nul
copy sysmodule.nsp Release\USB\atmosphere\titles\%TITLEID%\exefs.nsp
echo . > Release\USB\atmosphere\titles\%TITLEID%\flags\boot2.flag
7z a Release\USB.zip .\Release\USB\*

make clean
make DEFINES="-DRELEASE -DMODE_SOCKET" || goto quit
mkdir Release\Network\atmosphere\titles\%TITLEID% > nul
mkdir Release\Network\atmosphere\titles\%TITLEID%\flags > nul
copy sysmodule.nsp Release\Network\atmosphere\titles\%TITLEID%\exefs.nsp
echo . > Release\Network\atmosphere\titles\%TITLEID%\flags\boot2.flag
7z a Release\Networl.zip .\Release\Network\*

make clean
echo done.
:quit
pause
exit
