@echo off

set PATH=C:\Program Files\7zip;C:\Program Files\7-Zip\;C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\Tools\;%PATH%

echo Checking dependencies...
mkdir Resources\win-x64\native

:: Remember to also change the name of the folder in the zip file
if exist Resources\win-x64\native\avcodec-59.dll (
	goto skip_ffmpeg
)

echo Downloading ffmpeg binaries...
curl -L https://github.com/BtbN/FFmpeg-Builds/releases/download/autobuild-2023-02-28-12-37/ffmpeg-n5.1.2-39-g2953c6381a-win64-lgpl-shared-5.1.zip --output ffmpeg.zip
7z x ffmpeg.zip -olib_ffmpeg || goto error
:: Chage this when the download URL changes
copy lib_ffmpeg\ffmpeg-n5.1.2-39-g2953c6381a-win64-lgpl-shared-5.1\bin\*.dll Resources\win-x64\native\
del ffmpeg.zip

:skip_ffmpeg

:: cimgui_rX is the git tag for for the release we are usign of cimgui
if exist Resources\win-x64\native\cimgui_r2 (
	goto skip_cimgui_sdl
)

echo download cimgui-sdl2....
curl -L https://github.com/exelix11/CimguiSDL2Cross/releases/download/r2/windows-x64.zip --output cimgui-win-x64.zip
7z x cimgui-win-x64.zip -olib_cimgui || goto error
copy lib_cimgui\*.dll Resources\win-x64\native\
rmdir /s /q lib_cimgui
del cimgui-win-x64.zip
echo ok > Resources\win-x64\native\cimgui_r2

:skip_cimgui_sdl

if exist Resources\win-x64\native\SDL2_image.dll (
	goto skip_sdl_image
)

curl -L https://github.com/libsdl-org/SDL_image/releases/download/release-2.6.3/SDL2_image-2.6.3-win32-x64.zip --output SDL2_image.zip
7z x SDL2_image.zip -olib_SDL2_image || goto error
copy lib_SDL2_image\*.dll Resources\win-x64\native\
del SDL_image.zip

:skip_sdl_image

if exist Resources\win-x64\native\libusb-1.0.dll (
	goto skip_libusb
)

echo download libusb....
curl -L https://github.com/libusb/libusb/releases/download/v1.0.26/libusb-1.0.26-binaries.7z --output libusb.7z
7z x libusb.7z -olib_libusb || goto error
copy lib_libusb\libusb-1.0.26-binaries\VS2015-x64\dll\*.dll Resources\win-x64\native\
del libusb.7z

:skip_libusb

echo Building client...

cd ..
dotnet publish -c Release -r win-x64 /p:SysDvrTarget=windows || goto error

del bin\Release\net8.0\win-x64\publish\*.pdb

7z a Client.7z .\bin\Release\net8.0\win-x64\publish\*

:error
exit /B %ERRORLEVEL%
