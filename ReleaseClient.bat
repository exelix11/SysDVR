@echo off

set PATH=C:\Program Files\7zip;C:\Program Files\7-Zip\;C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\Tools\;%PATH%

REM Extract binaries if this is a CI build

if exist ffmpeg-master-latest-win64-lgpl-shared.zip (
	echo extracting ffmpeg-master-latest-win64-lgpl-shared.zip
	7z x ffmpeg-master-latest-win64-lgpl-shared.zip -offmpeg || goto error
	
	mkdir Client\runtimes\win-x64\native
	
	copy ffmpeg\ffmpeg-master-latest-win64-lgpl-shared\bin\*.dll Client\runtimes\win-x64\native
)

if exist wdi-simple.zip (
	echo extracting wdi-simple.zip
	7z x wdi-simple.zip -owdi-simple
	
	REM turns out we just need the 32bit version
	mkdir Client\runtimes\win\
	copy wdi-simple\wdi-simple32.exe Client\runtimes\win\
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
