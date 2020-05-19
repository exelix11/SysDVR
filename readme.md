# SysDVR
[![Discord](https://img.shields.io/discord/643436008452521984.svg?logo=discord&logoColor=white&label=Discord&color=7289DA
)](https://discord.gg/rqU5Tf8)
[![Latest release](https://img.shields.io/github/v/release/exelix11/SysDVR)](https://github.com/exelix11/SysDVR/releases)
[![Download](https://img.shields.io/github/downloads/exelix11/SysDVR/total)](https://github.com/exelix11/SysDVR/releases)

This is an experimental sysmodule that allows capturing the running game output to a pc via USB or network connection.

**Make sure to read the full readme, the tips and common issues sections in particular. If you need help ask on Discord/Gbatemp, use the issue tracker only for reporting actual bugs.**

## Features/Limitations
- **Video quality is fixed to 720p @ 30fps with h264 compression (hardware limit)**
- Audio quality is fixed to 16bit PCM @ 48kHz stereo. Not compressed
- **Only works on games that have video recording enabled** (aka you can long-press the capture button to save a video)
- Only captures game output. System UI, home menu and homebrews running as applet won't be captured
- Video feed is not realtime, there will always be a minimum of ~1 second of delay.
- Stream quality depends heavily on the environment, bad usb wires or low wifi signal can affect it significantly.
- USB streaming is not available when docked
- Requires at least firmware 6.0.0

Clearly with these limitations **this sysmodule doesn't allow "remote play" and does not fully replace a capture card**.

# Usage
This guide is for SysDVR V3 only, if you want to use an older version refer to the [previous version of the guide](https://github.com/exelix11/SysDVR/blob/7497d0961c194af0412e7c4338de8706c93c5fad/readme.md) 

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
There are 3 different streaming modes, each one with different strenghts and weaknesses here they're explained from the easiest to setup to the most involved.

To play the stream [mpv player](https://mpv.io/) is recommended as it's the most straight forward to set up, other players may work as well but I'm not supporting them.

**Streaming only works while a compatible game is running, this means that before before you can stream you must launch the game.**

For network streaming modes it's reccomended to use a **LAN adapter** or at least a 5GHz wifi, 2,4GHz is fine too as long as reception is **very good** for both switch and PC.
### Network RTSP mode
This is the simplest to setup, select `Stream over RTSP` in SysDVR Config and you're ready to go. To begin streaming open a terminal/command prompt in the mpv directory and run `mpv rtsp://<SwitchIpAddress>:6666/` , replace `<SwitchIpAddress>` with the ip address of your console (you can find it in the connection settings).

The RTSP server in SysDVR supports two modes, TCP and UDP, this usually doesn't matter but i've found different programs will connect using different modes, eg. mpv will always use TCP while obs will try UDP first and then TCP. Depending on your network environment you can find one to perform worse than the other so you'll have to test which one works best for you.

In mpv you can force udp mode by running `mpv rtsp://<SwitchIpAddress>:6666/ --rtsp-transport=udp`.

*Note for windows 10 users:* In the past i recommended to use windows 10 hotspot mode in case you don't have a LAN adapter, turns out that windows firewall will block UDP packets from the switch if streaming via UDP, as a workaround you can either stream only via TCP or **temporarily** disable windows firewall while streaming. 
### SysDVR-Client Setup
The following modes require SysDVR-Client (previously called UsbStream, it has been renamed as now it's used for one of the network modes as well).

Go to the [Releases tab](https://github.com/exelix11/SysDVR/releases) and download and extract the `SysDVR-Client.7z` file, it contains two executables:
- `SysDVR-Client.exe` that's the actual client, it runs on Windows, linux and mac. It's a command line application so may be a little hard to use for some users.
- `SysDVR-ClientGUI.exe` this is a graphical launcher for SysDVR-Client, this is Windows-only and it's meant to make things easier for less experienced users.

To run SysDVR-Client you need [.NET core 3](https://dotnet.microsoft.com/download), note it's not the same as .NET framework that you may already installed in the past.

**Main advantage of .NET Core is that it runs on linux and mac as well, so you can use SysDVR-Client on there too**.

If you're on Windows you can launch SysDVR-Client just by typing its name in the command prompt, for linux/mac users you'll have to type `dotnet UsbStream.dll` and append the arguments after that.

On Windows you can also use SysDVR-ClientGUI, it will automatically configure streaming modes from a user-friendly UI and create shortcuts for them. 

### Network TCP Bridge mode
In this mode SysDVR will send video and audio data to SysDVR-Client and it will relay both as an RTSP server.\
There are two advantages with this approach, first you can choose to stream only one channel (so it will get more bandwidth) and second all the RTSP overhead is handled by you pc so it does perform slightly better, especially when using a not very fast wifi.

To use this mode select `Stream over TCP Bridge` in SysDVR Config.

Now, if you're using SysDVR-ClientGUI, just select `TCP Bridge (Relay network via RTSP)`, type your switch IP address (found in the settings), select the mpv path and click launch. In a few seconds the stram should start.

For SysDVR-Client you'll have to launch it like this: `SysDVR-Client bridge <switch ip address>`, replace `<switch ip address>` with your console IP address no port is needed.\
Once it's running you can launch mpv from another terminal like this: `mpv rtsp://127.0.0.1:6666/` (this time `127.0.0.1` is the fixed local IP of your pc and you don't need to change it)

### USB streaming
To stream via USB you must select `Stream over USB` in SysDVR Config or download the USB-Only version.

Now to stream via USB you need to setup the custom driver. This step is needed only the first time and won't interfere with other USB homebrews as it uses a custom device ID.
#### Driver setup on windows
Plug your switch in the computer **while running SysDVR in USB mode** and launch [zadig](https://zadig.akeo.ie/), install the `libusb-win32` driver for the `SysDVR (Nintendo Switch)` device. **Before installing make sure the target device USB ID is `057e 3006`**, if it's different the sysmodule may not be running, try waiting a bit (it should start around 20 seconds after the console boot) or check again your setup.\
This won't interfere with other applications that communicate with the switch via usb as this sysmodule uses a different product id.

Libusb requires latest Microsoft C++ Redistributable Libs, they're often already installed on windows but in case of issues with Usb streaming you can download them from [here](https://support.microsoft.com/en-us/help/2977003/the-latest-supported-visual-c-downloads), depending on your Windows version you may need the x86 or x64 version (or both)
#### Driver setup on linux 
On linux you may have errors about loading the `libusb-1.0` library, this happens when the file name is different than the one expected by dotnet, you can make a symlink as described on the [LibUsbDotNet repo](https://github.com/LibUsbDotNet/LibUsbDotNet#linux-users):\
`sudo find / -name "libusb-1.0*.so*"` and then
```
cd /lib/x86_64-linux-gnu
sudo ln -s libusb-1.0.so.0 libusb-1.0.so
```
(Example commands, change the paths with the one you find on your pc)

#### Streaming
If you're using SysDVR-ClientGUI launch it, select `Stream via RTSP (Recommended)`, select the mpv path and click launch.

For linux users you can use the command line args:\
Launch `SysDVR-Client rtsp` to begin streaming, you can add the `--no-video` or `--no-audio` options to disable one of the streams. \
After that you just need to launch mpv like this: `mpv rtsp://127.0.0.1:6666/`(here too it's your pc's fixed ip and you don't need to change it) 

Streaming options for the previous version are still available but are *not recommended*, they're still shown in the GUI and you can see them by running `SysDVR-Client --help`

## Tips
To get the most out of this sysmodule you will have to do some testing to find configuration that works best for you, here are some tips and useful information:
- If the video stream doesn't start immediately try moving the camera around in the game, to display something the video player needs to acquire a video keyframe first.
- An usb 3 wire is slightly better than usb 2, but both should work.
- Quality also depends on the cpu utilization of the game you're running, OC can improve performances.
- Using cache *will* reduce lags at the cost of an higher delay, for mpv it's on by default, if you want to try without it the command line arg for mpv is `--cache=no --cache-secs=0`. There's a lot of room for experimenting here, try different values to find the combination that works best for you.
- Theoretically the best streaming setup would be having a direct lan connection switch to pc, without going through a router, this is likely not easy for the average user, may consider writing a guide for this but it's currently beyond the scope of SysDVR, in theory you'd need to host a dhcp server or fiddle with the static connection settings on switch and host your own 90dns instance locally.

## Common issues
**After copying the sysmodule to the sd atmosphere won't boot anymore** \
This happens if you have too many sysmodules or ones particulary heavy like sysftpd, remove them to launch SysDVR.\
If you really need other sysmodules at the same time you can try using the USB-Only version as it uses less memory (512K vs 3MB)\
To remove SysDVR delete the `/atmosphere/contents/00FF0000A53BB665` folder, to remove other sysmodules if you're not sure about the right ID delete the whole `/atmosphere` folder and download a fresh copy of atmosphere.

**Homebrews using USB like GoldLeaf or nxmtp won't launch anymore**\
This is because the USB interface is being used by SysDVR, if you're using the full version you can swith streaming modes using the SysDVR Settings homebrew included in the zip.

**When using network stream there's a lot of delay or it increses over time**\
Mpv will use a cache buffer to avoid lag when the connection drops some packets but this increases delay, you can disable it by using `--cache=no --cache-secs=0`, if this doesn't solve the issue likely your connection is not good enough for streaming.

**The video is laggy or there are a lot of glitches** \
Make sure the connection between the console and your pc is good enough, if streaming via network move both closer to the router or switch to LAN, for USB try using an higher quality wire.

# Extra
## Live streaming with OBS
This has been requested a lot, now that RTSP has been implemented you can just add SysDVR as a media source in OBS:
Add a new media source and untick local file, then just type `rtsp://<ip addr>:6666/` replace `<ip addr>` with the address you'd type in mpv following the guide for the streaming mode you want to use.

# Credits
- Everyone from libnx and the people who reversed grc:d and wrote the service wrapper, mission2000 in particular for the suggestion on how to fix audio lag.
- [mtp-server-nx](https://github.com/retronx-team/mtp-server-nx) for their usb implementation
- [RTSPSharp](https://github.com/ngraziano/SharpRTSP) for the C# RTSP library
- Bonta on discord for a lot of help implementing a custom RTSP server
