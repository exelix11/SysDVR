@echo off

set PATH=C:\Program Files\7zip;C:\Program Files\7-Zip\;C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\Tools\;%PATH%

cd Libs\LibUsbDotNet\src\LibUsbDotNet\
dotnet pack || goto error
move bin\Debug\LibUsbDotNet.*.nupkg ..\..\..\Built\

cd ..\..\..\..\Client.Native
dotnet pack || goto error
REM Client.Native is automatically copied to Libs\Built

cd ..\Client
dotnet publish -c Release || goto error

cd ..\ClientGUI
call VsDevCmd.bat
msbuild ClientGUI.csproj -p:Configuration=Release || goto error

cd ..

move Client\bin\Release\netcoreapp3.1\SysDVR-ClientGUI.exe Client\bin\Release\netcoreapp3.1\publish\SysDVR-ClientGUI.exe 

del Client\bin\Release\netcoreapp3.1\publish\*.pdb

7z a Client.7z .\Client\bin\Release\netcoreapp3.1\publish\*

:error
pause