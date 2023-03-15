@echo off

set PATH=C:\Program Files\7zip;C:\Program Files\7-Zip\;C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\Tools\;%PATH%

curl -L https://github.com/BtbN/FFmpeg-Builds/releases/download/autobuild-2023-02-28-12-37/ffmpeg-n5.1.2-39-g2953c6381a-win64-lgpl-shared-5.1.zip -O ffmpeg.zip

if exist ffmpeg.zip (
	echo extracting ffmpeg.zip
	7z x ffmpeg.zip -offmpeg || goto error
	
	mkdir Client\runtimes\win-x64\native
	
	copy ffmpeg\ffmpeg-master-latest-win64-lgpl-shared\bin\*.dll Client\runtimes\win-x64\native
)

cd Client
dotnet publish -c Release || goto error

call VsDevCmd.bat

cd ..
nuget restore

cd ClientGUI
dotnet publish -c Release || goto error

cd ..

move ClientGUI\bin\Release\net4.5.1\publish\* Client\bin\Release\net6.0\publish\ 

del Client\bin\Release\net6.0\publish\*.pdb

7z a Client.7z .\Client\bin\Release\net6.0\publish\*

:error
exit /B %ERRORLEVEL%
