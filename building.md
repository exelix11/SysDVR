# Compiling SysDVR

This file explains how to build SysDVR from source locally. If you are looking for prebuilt binaries, check out the [releases page](https://github.com/exelix11/SysDVR/releases/).

These instructions were written as of SysDVR v6.0, for previous or future versions you can verify any changes in the [CI build scripts](https://github.com/exelix11/SysDVR/tree/master/.github/workflows) which should always be up to date.

The SysDVR project is composed of three components that must be buit individually, these are the [SysDVR sysmodule](https://github.com/exelix11/SysDVR/tree/master/sysmodule), its [Settings homebrew](https://github.com/exelix11/SysDVR/tree/master/SysDVRConfig) and [SysDVR client](https://github.com/exelix11/SysDVR/tree/master/Client).

# Sysmodule and Settings

These are regular switch homebrew projects, you can build them using the devkitA64 toolchain from DevKitPro.

Switch projects use a `Makefile` to build, there are certain special compliation flags that can be used to toggle SysDVR features, the most relevant is `USB_ONLY`, to be set by runnin `make -j DEFINES="-DUSB_ONLY"` which produces the USB-only version of SysDVR.

There are other flags meant for debugging, these are not stable and not documented here.

# Client

The client is a cross platform C# project that runs on Windows, Linux, MacOS (both intel and arm) and Android.

The wide range of supported platforms causes the build process to be a bit more complex than the sysmodule.

The client is built as a NativeAOT application, meaning that each platform gets its own native executable that can be launched without the need for the dotnet runtime.
The following platforms are distributed as NativeAOT binaries:

- Windows x64
- Linux x64 (flatpak)
- MacOS x64 and arm64
- Android arm64

To support other platforms, most notably Linux on arm such as the Raspberry Pi, you will need to build the client from source.

The build script for each platform is located in the Client/Platform folder.

## Build tools

SysDVR requires dotnet 8.0 to build, the android version additionaly requires the native android SDK and android studio.

## Client build

The client uses a .NET csproj to build, using the dotnet CLI should be enough to build a managed (requires dotnet runtime) version of the client. This is ideal for development as, once you have the native libraries installed, you can just run and debug the client from Visual Studio or VSCode.

The following are the build scripts for each platform, they are also tasked to download the native libraries:
- [Windows](https://github.com/exelix11/SysDVR/blob/master/Client/Platform/BuildWindows.bat)
- [MacOs](https://github.com/exelix11/SysDVR/blob/master/Client/Platform/BuildMacos.sh)
- [Linux](https://github.com/exelix11/SysDVR/blob/master/Client/Platform/Linux/build-flatpak.sh) (flatpak, will compile the dependencies from source)
- [Android](https://github.com/exelix11/SysDVR/blob/master/Client/Platform/Android/buildbinaries.sh)

Android builds are special as they require the additional `/p:SysDvrTarget=android` parameter for some platform specific code to be included. This is an option for other platforms as well but as of now it is not used, you should use the current build scripts as reference.
Furthermore, the android build process requires to use gradle or android studio to build the final APK, the `buildbinaries.sh` script will only prepare dependencies and the SysDVR client AOT library.

## Dependencies 

Before being able to run the compiled client, regardless of managed or aot builds, you will need to obtain the executable files for all the native dependencies the client uses.

These are:
- ffmpeg (libavcodec, libavformat, libavutil, libswscale, etc)
- SDL2
- SDL_image
- LibUSB (only if you plan to use usb streaming)
- [CimguiSDL2Cross](https://github.com/exelix11/CimguiSDL2Cross)
	- This is a fork of the open source cimgui project with some SDL-specific tweaks. It is built by the CI on that repo and i release the binary packages so they can be downloaded by the build scripts.

Since asking non-windows users to properly install these libraries is essentially impossible, we bundle all the native libraries in the client releases. These are downloaded in the `Platform/runtimes/your_platform/native` folder (for example `runtimes/linux-arm64/native` for 64-bit arm linux) and the msbuild compilation process will automatically include them in the final executable folder. 

If you are planning to run the client on your own pc you can simply download the needed libraries from your package manager and run sysdvr without copying the binaries, the native folder is only the _preferred_ load location and if the client fails to find the libraries there it will fallback to the system path.

The only exception is for CimguiSDL2Cross which must be self built since it is completely custom, you should be able to build it with cmake following the [instructions](https://github.com/exelix11/CimguiSDL2Cross/tree/master/cimgui#compilation) on the repo itself. You can use SysDVR without CimguiSDL2Cross if you run with the `--legacy` option but you will lose the GUI.

You can debug library loading issues by launching the client with the `--debug dynlib` argument.

## Dependencies for Android

Android builds require the APK to include the native libraries, we use the android NDK preferred way of doing this by copying them to the `app/jni` folder, some are built from source while others are downloaded from github build feeds.
