# SysDVR
This is an experimental sysmodule that allows capturing the running game output to a pc.\
Two versions are provided: one to stream via USB and one to stream over the network (TCP). Currently it's not possible to use both at the same time.\
Using [mpv player](https://mpv.io/) is recommended as it's the most straight forward to set up, any other player that supports raw h264 streams via tcp or stdin should work but you may have to configure it manually.
## Limitations
- **Video quality is fixed to 720p @ 30fps with h264 compression (hardware limit)**
- Audio quality is fixed to 16bit PCM @ 48kHz stereo. Not compressed
- **Only works on games that have video recording enabled** (aka you can long-press the capture button to save a video)
- Video and audio are two different streams, they're likely to desync as they require two different player instances. Vlc does support a secondary audio stream but i didn't manage to get it working properly.
- Only captures game output. System UI, home menu and homebrews running as applet won't be captured
- Video feed is not realtime, there will always be a minimum of ~1 second of delay.
- Stream quality depends heavily on the environment, bad usb wires or low wifi signal can affect it significantly.
- Stream quality is also affected by software configuration, more details at the bottom.
- USB streaming is not available when docked
- Requires firmware >= 6.0.0

Clearly with these limitations **this sysmodule doesn't allow "remote play" and does not replace a capture card**.
## Usage
### Setting up the sysmodule
The provided builds already contain the correct file structure, you should just be able to extract them to your sd card.\
To remove the sysmodule just delete the `atmosphere/titles/00FF0000A53BB665` folder from your sd card.\
CFWs other than atmosphere should work but i won't provide support for them.
### Network streaming
This is the easiest way to stream, In this mode the sysmodule is completely standalone, you should be able to play the video stream just by running `mpv tcp://<switch ip address>:6666 --no-correct-pts --fps=30 ` and `mpv tcp://<switch ip addr>:6667 --no-video --demuxer=rawaudio --demuxer-rawaudio-rate=48000` for audio.\
When using network streaming it's not recommended to stream both audio and video at the same time as it may cause slowdowns.
*In general network streaming has more lag and delay compared to USB, it is worth the time to set it up, especially for gameplay recording.*
### USB streaming
To stream via usb you need the UsbStream program, it's built using dotnet core 3.0 and is compatible with linux as well.\
First of all you should make sure your drivers are set up correctly (only the first time) :
#### Driver setup on windows
On windows you may get the `device not found` or `unsupported driver` errors or very bad performance on the stream, in this case you may have the wrong driver set up, download zadig and install the `libusb-win32` driver for the "Nintendo Switch" device. **Before installing make sure the target device USB ID is `057e 3006`**, if it's different the sysmodule may not be running, try waiting a bit (it should start around 20 seconds after the console boot) or check again your setup. This won't interfere with other application that communicate with the switch via usb as this sysmodule uses a different product id.
#### Driver setup on linux 
On linux you may have errors about loading the `libusb-1.0` library, this happens when the file name is different than the one expected by dotnet, you can make a symlink as described on the [LibUsbDotNet repo](https://github.com/LibUsbDotNet/LibUsbDotNet#linux-users):\
`sudo find / -name "libusb-1.0*.so*"` and then
```
cd /lib/x86_64-linux-gnu
sudo ln -s libusb-1.0.so.0 libusb-1.0.so
```
(Example commands, change the paths with the one you find on your pc)
#### Streaming
UsbStream supports three streaming modes: 
- `mpv` or `stdin` : pipes the received data directly to a video player via stdin, this will use no caching so you'll have low delay but it may lag. Using `mpv` will automatically add the needed configuration for mpv, `stdin` will require you to type the args to pass to the player
- `tcp` : opens a tcp server so players can connect to it and use their own caching mechanism to remove or reduce lag (at the price of an higher delay)
- `file` : writes the received data directly to a file so it can be converted to a common format later.

Launch UsbStream like this:
`UsbStream video <streaming mode> <arg> audio <streaming mode> <arg>` \
When using `mpv` the arg is the mpv executable (.com file on windows) path, you have to repeat it twice if using both streams. \
The `tcp` option requires a free port number and the `file` option the output file path. \
To disable a stream just omit the name and its fields.\
Example commands:
```
UsbStream audio mpv C:/programs/mpv/mpv : Plays audio via mpv located at C:/programs/mpv/mpv, video is ignored
UsbStream video mpv ./mpv audio mpv ./mpv : Plays video and audio via mpv (path has to be specified twice)
UsbStream video tcp 1337 audio file C:/audio.raw : Streams video over port 1337 while saving audio to disk
```
Note that on windows you should use the mpv.com file and not mpv.exe, omitting the extension will automatically use the right one
Launching UsbStream without any parameter will display more options and examples.\
To connect to the tcp streams you can use: `mpv tcp://localhost:<video port> --no-correct-pts --fps=30 ` for video and `mpv tcp://localhost:<audio port> --no-video --demuxer=rawaudio --demuxer-rawaudio-rate=48000` for audio
## Tips
To get the most out of this sysmodule you have to do a lot of testing to find the best configuration that works for you, here are some tips and answers to common issues :
- If the video stream doesn't start immediately try moving the camera around in the game, to display something the video player needs to acquire a video keyframe first
- If the stream is very poor try a different usb cable and driver with zadig, for network stream bring your console closer to the router, try using a lan adapter or windows' own hotspot mode.
- An usb 3 wire is slightly better than usb 2, but both should work.
- Quality also depends on the cpu utilization of the game you're running, OC can improve performances.
- Using cache *will* reduce lags at the cost of an higher delay, for mpv it's on by default (except when using usb stream with `mpv` mode), if you want to try without it the command line arg for mpv is `--cache=no --cache-secs=0`. There's a lot of room for experimenting here, try different values to find the combination that works best for you.
- If using no caching when the screen goes full black or white (eg loading screens) it may cause some desync and the stream will start to drop frames, eventually it will fix by itself but it could take some time, the fastest solution is to open the home menu for a few seconds and then resume the game, for usb stream you can try setting the `--desync-fix` flag (this will introduce glitches).
## Known issues/TODOs
- Memory usage is kept as low as possible (512K for usb ver, 3MB for network) but running this with many other sysmodules, or ones particulary heavy like sysftpd will not work or hang your console on boot.
- The usb version stops working when launching another homebrew that requires usb access like nxmtp or goldleaf.
  - Implement a key combination to reinitialize usb when this happens (?)
- Merge the usb and network stream versions and make an homebrew app to toggle between the two 
- Improve automatic desync detection and fix for UsbStream

## Credits
- Everyone from libnx and the people who reversed grc:d and wrote the service wrapper, mission2000 in particular for the suggestion on how to fix audio lag.
- [mtp-server-nx](https://github.com/retronx-team/mtp-server-nx) for their usb implementation
