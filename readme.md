# SysDVR
[![Discord](https://img.shields.io/discord/643436008452521984.svg?logo=discord&logoColor=white&label=Discord&color=7289DA
)](https://discord.gg/rqU5Tf8)
[![Latest release](https://img.shields.io/github/v/release/exelix11/SysDVR)](https://github.com/exelix11/SysDVR/releases)
![](https://img.shields.io/github/downloads/exelix11/SysDVR/total)
[![Contributing](https://img.shields.io/badge/supporting-patreon-f96854)](https://www.patreon.com/exelix11)

This is a WIP readme/guide for SysDVR V3, if you're looking for latest stable version head to the [readme file of the master branch](https://github.com/exelix11/SysDVR/blob/master/readme.md) 

This is an experimental sysmodule that allows capturing the running game output to a pc via USB or network connection.\
Before opening issues make sure to read the full readme, the tips and commmon issues sections in particular.

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

## Supporting the project
If you like my work consider supporting me on [patreon](https://www.patreon.com/exelix11)

# Usage
## Setting up the sysmodule
Before continuing make sure you're using latest version of atmosphere (other CFWs should work but I won't provide support for them)

Two versions are provided in the [Releases tab](https://github.com/exelix11/SysDVR/releases):
- `SysDVR.zip` is the "full" version, can stream using both modes and comes with an homebrew app to switch between them
- `SysDVR-USB-Only.zip` will only stream via USB but uses less memory and should run alongside multiple sysmodules.

You should use the full version unless you have specific requirements.\
Download one of the builds and extract it in the root of your sd card, the zips already contain the correct file structure.\
If you added the module with the console on, you will need to reload your CFW for the new module to be loaded. This can normally be accomplished by rebooting from the Power menu.

**By default SysDVR will stream over network**, to switch between modes and set the default one you can use the SysDVR Settings homebrew included in the zip.
## Streaming modes
There are 3 different streaming modes, each one with different strenghts and weaknesses here they're explained from the easiest to setup to the most involved.

To play the stream [mpv player](https://mpv.io/) is recommended as it's the most straight forward to set up, any other player that supports raw h264 streams via tcp or stdin should work but you may have to configure it manually.

**Streaming only works while a compatible game is running, this means that before before you can stream you must launch the game.**

For network streaming modes it's reccomended to use a **LAN adapter** or at least a 5GHz wifi.
### Network RTSP mode
This is the simplest to setup, select `Stream over RTSP` in SysDVR Config and you're ready to go. To begin streaming open a terminal/command prompt in the mpv directory and run `mpv rtsp://<SwitchIpAddress>:6666/` , replace `<SwitchIpAddress>` with the ip address of your console (you can find it in the connection settings).

The RTSP server in SysDVR supports two modes, TCP and UDP, this usually doesn't matter but i've found different programs will connect using different modes, eg. mpv will always use TCP while obs will try UDP first and then TCP. Depending on your network environment you can find one to perform worse than the other so you'll have to test which one works best for you.

In mpv you can force udp mode running `mpv rtsp://<SwitchIpAddress>:6666/ --rtsp-transport=udp`, similarly you can force TCP mode in obs by typing `--rtsp-transport=tcp` in the file format field.

*Note for windows 10 users:* In the past i recommended to use windows 10 hotspot mode in case you don't have a LAN adapter, turns out that windows firewall will block UDP packets from the switch if streaming via UDP, as a workaround you can either stream only via TCP or **temporarily** disable windows firewall while streaming. 
### Network TCP Bridge mode
In this mode SysDVR will send video and audio data to USBStream ~~likely going to change its name cause this feature isn't actually USB~~ and it will relay it as an RTSP server.\
There are two advantages with this approach, first you can choose to stream only one channel (so it will get more bandwidth) and second all the RTSP overhead is handled by you pc so it does perform slightly better, especially when using a not very fast wifi.

To use this mode select `Stream over TCP Bridge` in SysDVR Config, then launch UsbStream (Check the USB Streaming part for requirements) like this: `UsbStream bridge <switch ip address>` no port is needed.\
Once it's running you can launch mpv like this: `mpv rtsp://127.0.0.1:6666/` (this time `127.0.0.1` is the fixed IP of your pc and you don't need to change it)
### USB streaming
To stream via USB you must select `Stream over USB` in SysDVR Config or download the USB-Only version.
### USBStream requirements
You'll need the UsbStream program, it's built using .NET core and is compatible with linux as well.\
First of all you should make sure to install [.NET core 3](https://dotnet.microsoft.com/download) on your pc. \
Then if you're on linux you can launch UsbStream from terminal like this: `dotnet UsbStream.dll` and append the arguments after that, on Windows just typing UsbStream in the cmd is enough.\
Now to stream via USB you need to setup the custom driver. This step is needed only the first time and won't interfere with other USB homebrews as it uses a custom device ID.
#### Driver setup on windows
On windows you may get the `device not found` or `platform not supported` errors or very bad performance on the stream, in this case you may have the wrong driver set up.\
Plug your switch in the computer **while running SysDVR in USB mode** and launch [zadig](https://zadig.akeo.ie/) install the `libusb-win32` driver for the "Nintendo Switch" device. **Before installing make sure the target device USB ID is `057e 3006`**, if it's different the sysmodule may not be running, try waiting a bit (it should start around 20 seconds after the console boot) or check again your setup.\
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
Most Windows users can simply use UsbStreamGUI, it will allow you to select the streaming mode and launch UsbStream directly.\
For linux users you can use the command line args:\
Just launching UsbStream will show a brief help message and attempt to start USB streaming.\
You can skip the message by launching `UsbStream rtsp`\
You can use the `--no-video` or `--no-audio` options to disable one of the streams. \
After that you just need to launch mpv like this: `mpv rtsp://127.0.0.1:6666/`(here too it's your pc's fixed ip and you don't need to change it) \
The previous streaming options are still available but are not recommended, you can see them by running `UsbStream --help`
## Tips
To get the most out of this sysmodule you will have to do some testing to find configuration that works best for you, here are some tips and useful information:
- If the video stream doesn't start immediately try moving the camera around in the game, to display something the video player needs to acquire a video keyframe first.
- An usb 3 wire is slightly better than usb 2, but both should work.
- Quality also depends on the cpu utilization of the game you're running, OC can improve performances.
- Using cache *will* reduce lags at the cost of an higher delay, for mpv it's on by default (except when using usb stream with `mpv` mode), if you want to try without it the command line arg for mpv is `--cache=no --cache-secs=0`. There's a lot of room for experimenting here, try different values to find the combination that works best for you.
- If using no caching when the screen goes full black or white (eg loading screens) it may cause some desync and the stream will start to drop frames, eventually it will fix by itself but it could take some time, the fastest solution is to open the home menu for a few seconds and then resume the game.
## Common issues
**After copying the sysmodule to the sd atmosphere won't boot anymore** \
This happens if you have too many sysmodules or ones particulary heavy like sysftpd, remove them to launch SysDVR.\
If you really need other sysmodules at the same time you can try using the USB-Only version as it uses less memory (512K vs 3MB)\
To remove SysDVR delete the `/atmosphere/contents/00FF0000A53BB665` folder, to remove other sysmodules if you're not sure about the right ID delete the whole `/atmosphere` folder and download a fresh copy of atmosphere.

**Homebrews using USB like GoldLeaf or nxmtp won't launch anymore**\
This is because the USB interface is being used by SysDVR, if you're using the full version you can swith streaming modes using the SysDVR Settings homebrew included in the zip.

**When using network stream there's a lot of delay or it increses over time**\
Mpv will use a cache buffer to avoid lag when the connection drops some packets but this increases delay, you can disable it by using `--cache=no --cache-secs=0`

**The video is laggy or there are a lot of glitches** \
Make sure the connection between the console and your pc is good enough, if streaming via network move both closer to the router or switch to LAN, for USB try using an higher quality wire.

## Credits
- Everyone from libnx and the people who reversed grc:d and wrote the service wrapper, mission2000 in particular for the suggestion on how to fix audio lag.
- [mtp-server-nx](https://github.com/retronx-team/mtp-server-nx) for their usb implementation
- [RTSPSharp](https://github.com/ngraziano/SharpRTSP) for the C# RTSP library
- Bonta on discord for a lot of help implementing a custom RTSP server
