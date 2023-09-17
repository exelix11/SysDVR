# SysDVR-Client for Android

Since I don't want to maintain a separate codebase just for android I decided to try making a new cross platform Client GUI that runs everywhere. This is made possible thanks to the excellent SDL android support which is the engine that runs everything.

The android build process is rather convoluted, in short:
1) Build SysDVR-Client as a native AOT library
  - currently this uses the dotnet 8 preview, we'll move to stable as soon as it's out.
2) Gather all the native libraries as source code
3) Build the whole thing as an android app with gradle/android studio

Since to the android source sysdvr is just an opaque native library it must be built separately, this is done using the `buildbinaries.sh` script.

More info on the "framework" I'm using to build this: https://github.com/exelix11/CimguiSDL2Cross
