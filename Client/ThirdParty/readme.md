# ThirdParty folder

This folder contains third party dotnet libraries that are needed for SysDVR-Client.

This is done because none of these libraries support android at the time of writing so their sources here are updated for the new dotnet 7+ apis for native library loading.

The following is a non-exhaustive list of changes:
- Added support for [android native wrappers](https://github.com/libusb/libusb/wiki/Android) in libusbdotnet which use a different api than normal libusb
- Added support for android native library loading to FFmpeg.AutoGen
- Added pinvoke wrappers for certain SDL backend and internal imgui functions to ImGui.NET
- Added pinvoke wrappers for certain android-only SDL apis in SDL2-CS-master

Also nuget versions come with native libraries which is usually good but since we strive for cross platform we inevitably need to do some dependency management manually.
Having all the libraries as part of the source tree completely removes native binaries so we can provde our own.
