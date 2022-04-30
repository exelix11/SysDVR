These are the third party libs used by SysDVR. Some are built as nuget packages are they carry native dependencies and that's the only way to make dotnet automatically copy and load them from the runtimes folder

The submodules are forks i made for this project with the following changes:
- Build as nupkg and remove NLog dependency from SharpRTSP
- Pinvoke to load libraries from the proper folders on linux and windows for Ffmpeg.Autogen
