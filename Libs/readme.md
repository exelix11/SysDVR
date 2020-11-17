These are the third party libs used by SysDVR. Some are built as nuget packages are they carry native dependencies and that's the only way to make dotnet automatically copy and load them from the runtimes folder

The libs added as submodules are just to keep track of the source, they're forks i made to do minor changes:
- Linux arm support for libusbdotnet
- Build as nupkg and remove NLog dependency from SharpRTSP
- Pinvoke to load libraries from the proper folders on linux and windows for Ffmpeg.Autogen