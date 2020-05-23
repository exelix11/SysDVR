# SysDVR
[![Discord](https://img.shields.io/discord/643436008452521984.svg?logo=discord&logoColor=white&label=Discord&color=7289DA
)](https://discord.gg/rqU5Tf8)
[![Latest release](https://img.shields.io/github/v/release/exelix11/SysDVR)](https://github.com/exelix11/SysDVR/releases)
[![Downloads](https://img.shields.io/github/downloads/exelix11/SysDVR/total)](https://github.com/exelix11/SysDVR/releases)
[![ko-fi](https://www.ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/K3K11QRUS)

This is an experimental sysmodule that allows capturing the running game output to a pc via USB or network connection.

**If you have issues make sure to read the full readme, the troubleshooting section in particular. If you need help you can either ask on discord or open an issue with the correct template.**

## Features/Limitations
- **Video quality is fixed to 720p @ 30fps with h264 compression, this is an hardware limit**
- Audio quality is fixed to 16bit PCM @ 48kHz stereo. Not compressed
- **Only works on games that have video recording enabled** (aka you can long-press the capture button to save a video)
- Only captures game output. System UI, home menu and homebrews running as applet won't be captured
- It is not possible to stream audio in real-time, low-latency video works fine
- Stream quality depends heavily on the environment, bad usb wires or low wifi signal can affect it significantly.
- **USB streaming is not available when docked**
- Requires at least firmware 6.0.0

Clearly with these limitations **this sysmodule doesn't fully replace a capture card**.

### Donation
If you like my work and wish to support me you can donate on [ko-fi](https://ko-fi.com/exelix11)

# Usage
This guide is for SysDVR 4.0 only, if you want to use an older version refer to the [previous versions of the guide](https://github.com/exelix11/SysDVR/commits/master/readme.md) 

## Setting up the sysmodule
Before continuing make sure you're using latest version of atmosphere (other CFWs should work but I won't provide support for them)

Then go to the [Releases tab](https://github.com/exelix11/SysDVR/releases), you'll find the following releases:
- `SysDVR.zip` is the "full" version, can stream using both network and USB modes and comes with an homebrew app to switch between them
- `SysDVR-USB-Only.zip` will only stream via USB but uses less memory and should run alongside multiple sysmodules.

You should use the full version unless you have specific requirements.

Download one of the builds and extract it in the root of your sd card, the zips already contain the correct file structure.\
If you added the module with the console on, you will need to restart your console for the module to be loaded.

**By default SysDVR will stream over RTSP (network)**, to switch between modes and set the default one you can use the SysDVR Settings homebrew included in the zip.
## Streaming modes
There are three different streaming modes, each one with different strenghts and weaknesses here they're explained from the easiest to setup to the most involved.

To play the stream [mpv player](https://mpv.io/) is recommended as it's the most straight forward to set up, other players may work as well but I'm not supporting them.

**Streaming only works while a compatible game is running, launch it before starting to stream. In the home menu streaming won't work.**

For network streaming modes it's reccomended to use a **LAN adapter** or at least a 5GHz wifi, 2.4GHz is fine too as long as reception is **very good** for both switch and PC.
### Network RTSP mode (default mode)
This is the simplest to setup but requires a very strong connection between the PC and your console, select `Stream over RTSP` in SysDVR Config and you're ready to go.\
To begin streaming open a terminal/command prompt in the mpv directory and run `mpv rtsp://<SwitchIpAddress>:6666/` , replace `<SwitchIpAddress>` with the IP address of your console (you can find it in the connection settings).

*Note for windows 10 users:* In the past i recommended to use windows 10 hotspot mode in case you don't have a LAN adapter, turns out that windows firewall may block the data from the switch when using certain players, as a workaround you can try a different player or **temporarily** disable windows firewall while streaming. 

### SysDVR-Client Modes
The following modes require SysDVR-Client, a cross-platform PC app to receive and relay the stream.

Go to the [Releases tab](https://github.com/exelix11/SysDVR/releases), download and extract the `SysDVR-Client.7z` file, among the various files it contains two executables:
- `SysDVR-Client.exe` that's the actual client. It's a command line application so may be a little hard to use for some users.
- `SysDVR-ClientGUI.exe` this is a graphical launcher for SysDVR-Client, this is Windows-only and it's meant to make things easier for less experienced users.

To run SysDVR-Client you need [.NET core 3](https://dotnet.microsoft.com/download), note that it's not the same as .NET framework that you may have already installed in the past.

If you're on Windows you can launch SysDVR-Client just by typing its name in the command prompt, on linux and mac you'll have to type `dotnet UsbStream.dll` and append the arguments after that.

On Windows you can also use SysDVR-ClientGUI, it will automatically configure streaming modes from a user-friendly UI and create shortcuts for them. 

You can see all the streaming options by launching `SysDVR-Client --help`

### Network TCP Bridge mode
In this mode SysDVR will send video and audio data to SysDVR-Client and it will relay both as an RTSP server.\
There are two advantages with this approach, first you can choose to stream only one channel (so it will get more bandwidth) and second all the RTSP overhead is handled by you pc so it does perform better, especially when the connection isn't excellent.

To use this mode select `Stream over TCP Bridge` in SysDVR Config.

Now, if you're using SysDVR-ClientGUI, just select `TCP Bridge (network mode)`, type your switch IP address select the mpv path and click launch. In a few seconds the stream should start.

For SysDVR-Client you'll have to launch it like: `SysDVR-Client bridge <switch ip address>`, replace `<switch ip address>` with your console IP address, no port is needed.\
Once it's running you can launch mpv from another terminal like this: `mpv rtsp://127.0.0.1:6666/` (this time `127.0.0.1` is the fixed local IP of your PC and you don't need to change it)

### USB streaming
To stream via USB select `Stream over USB` in SysDVR Config or download the USB-Only version.

Before you can streem have to setup the custom driver. This step is needed only the first time and won't interfere with other USB homebrews as it uses a custom device ID. \
If you did this before version 4.0 you must do it again as the USB implementation changed to improve performances.
#### Driver setup on windows
Plug your switch in the computer **while running SysDVR in USB mode** and launch [zadig](https://zadig.akeo.ie/), install the `WinUSB` driver for the `SysDVR (Nintendo Switch)` device. **Before installing make sure the target device USB ID is `057e 3006`**, if it's different the sysmodule may not be running, try waiting a bit (it should start around 20 seconds after the console boot) or check again your setup.\
If you installed the libusb driver previously and don't see `SysDVR (Nintendo Switch)` in the devices list click on Options and List all devices.\
This won't interfere with other applications that communicate with the switch via usb as this sysmodule uses a different product id.

#### Driver setup on linux 
On linux you may have errors about loading the `libusb-1.0` library, this happens when the file name is different than the one expected by dotnet, you can make a symlink as described on the [LibUsbDotNet repo](https://github.com/LibUsbDotNet/LibUsbDotNet#linux-users):\
`sudo find / -name "libusb-1.0*.so*"` and then
```
cd /lib/x86_64-linux-gnu
sudo ln -s libusb-1.0.so.0 libusb-1.0.so
```
(Example commands, change the paths with the one you find on your pc)

#### Streaming
If you're using SysDVR-ClientGUI select `USB (requires setting up USB drivers)`, select the mpv path and click launch.

For linux users you can use the command line args:\
Launch `SysDVR-Client usb` to begin streaming, you can add the `--no-video` or `--no-audio` options to disable one of the streams. \
After that you just need to launch mpv like this: `mpv rtsp://127.0.0.1:6666/`(here too it's your pc's fixed ip and you don't need to change it) 

## Low-latency streaming
By default SysDVR-Client will stream over RTSP to have a synchronized audio and video feed. It's possible to disable synchronization of the streams to have a low-latency stream.

To stream the video or audio feed directly to MPV in the GUI you can select the `Play in mpv` option, for the command line interface add `--mpv <path to mpv executable>` at the end. This will automatically launch mpv.

**Only one stream is supported in this mode, by default audio gets disabled.**

## Troubleshooting

**USB streaming and mpv don't work, they complain about missing DLLs or nothing happens**\
Try installing the [Microsoft VC++ libs](https://support.microsoft.com/en-us/help/2977003/the-latest-supported-visual-c-downloads), depending on your Windows version you may need the x86 or x64 version (or both)

**Streaming via USB doesn't work, I get timeout errors**\
Make sure you installed the `WinUsb` driver, the previous versions of SysDVR required `libusb-win32` but it has been changed to improve performances, you'll have to setup the driver again. If it still doesn't work try using the `Force LibUsb backend` option in the GUI or `--no-winusb` command line argument

**After copying the sysmodule to the sd atmosphere won't boot anymore** \
This happens if you have too many sysmodules or ones particulary heavy like sysftpd, remove them to launch SysDVR.\
If you really need other sysmodules at the same time you can try using the USB-Only version as it uses less memory (512K vs 1MB)\
To remove SysDVR delete the `/atmosphere/contents/00FF0000A53BB665` folder, to remove other sysmodules if you're not sure about the right ID delete the whole `/atmosphere` folder and download a fresh copy of atmosphere.

**Homebrews using USB like GoldLeaf or nxmtp won't launch anymore**\
This is because the USB interface is being used by SysDVR, if you're using the full version you can swith streaming modes using the SysDVR Settings homebrew included in the zip.

**When using network stream there's a lot of delay or it increses over time**\
Mpv will use a cache buffer to avoid lag when the connection drops some packets but this increases delay, you can disable it by using `--cache=no --cache-secs=0`, if this doesn't solve the issue likely your connection is not good enough for streaming.

**The video is laggy or there are a lot of glitches** \
Make sure the connection between the console and your pc is good enough, if streaming via network move both closer to the router or switch to LAN, for USB try using an higher quality wire.

**The SysDVR-settings app says that SysDVR isn't running but i've set it up correctly**\
If you're using the full version SysDVR launches ~20 seconds after console boot, if you launch the settings up too quickly it won't connect, wait a bit and try again.
SysDVR USB-Only version doesn't support the settings app.

**Streaming doesn't work, the GUI launches a single black window, if I double click SysDVR-Client.exe nothing happens**\
You didn't install the correct version of .NET core, try installing [this version](https://dotnet.microsoft.com/download/dotnet-core/thank-you/runtime-desktop-3.1.3-windows-x64-installer), if it still doesn't work you can launch the client manually using `dotnet SysDVR-Client.dll`

# Extra
## Live streaming with OBS
This has been requested a lot, now that RTSP has been implemented you can just add SysDVR as a media source in OBS:
Add a new media source and untick local file, then just type `rtsp://<ip addr>:6666/` replace `<ip addr>` with the address you'd type in mpv following the guide for the streaming mode you want to use.

## Advanced tips
- Quality also depends on the cpu utilization of the game you're running, OC could improve performances.
- For direct RTSP mode the SysDVR server supports two modes: TCP and UDP, this usually doesn't matter but i've found different programs will connect using different modes, eg. mpv will always use TCP while obs will try UDP first and then TCP. Depending on your network environment you can find one to perform worse than the other so you'll have to test which one works best for you. In mpv you can force udp mode by running `mpv rtsp://<SwitchIpAddress>:6666/ --rtsp-transport=udp`.
- It's possible to stream RTSP in low latency mode as well from mpv using the following command line options: `--profile=low-latency --no-cache --cache-secs=0 --demuxer-readahead-secs=0 --untimed --cache-pause=no --no-correct-pts` this will disable streams synchronization
- Theoretically the best streaming setup would be having a direct lan connection switch to pc, without going through a router, this is likely not easy for the average user, may consider writing a guide for this but it's currently beyond the scope of SysDVR, in theory you'd need to host a dhcp server or fiddle with the static connection settings on switch and host your own 90dns instance locally.

## Unfinished stuff
At some point I tried to implement a usb video class device via software, this would have allowed for usb streaming without needing a custom client app. Unfortunately this didn't work out, if you have more experience than me in this stuff feel free to give it a try, you can find more info in the readme of the UVC branch. 

# Credits
- Everyone from libnx and the people who reversed grc:d and wrote the service wrapper, mission2000 in particular for the suggestion on how to fix audio lag.
- [mtp-server-nx](https://github.com/retronx-team/mtp-server-nx) for their usb implementation
- [RTSPSharp](https://github.com/ngraziano/SharpRTSP) for the C# RTSP library
- Bonta on discord for a lot of help implementing a custom RTSP server
- @xerpi for a lot of help while working on the UVC branch