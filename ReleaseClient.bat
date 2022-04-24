@echo off

set PATH=C:\Program Files\7zip;C:\Program Files\7-Zip\;C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\Tools\;%PATH%

cd Libs\Client.Native
dotnet build -c Release || goto error
REM Client.Native is automatically copied to Libs\Built

cd ..\..\Client
dotnet publish -c Release || goto error

cd ..\ClientGUI
call VsDevCmd.bat
msbuild ClientGUI.csproj /p:Configuration=Release || goto error

cd ..

move Client\bin\Release\net6.0\SysDVR-ClientGUI.exe Client\bin\Release\net6.0\publish\SysDVR-ClientGUI.exe 

del Client\bin\Release\net6.0\publish\*.pdb

7z a Client.7z .\Client\bin\Release\net6.0\publish\*

:error
exit /B %ERRORLEVEL%