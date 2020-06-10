@echo off

set PATH=C:\Program Files\7zip;C:\Program Files\7-Zip\;C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\Tools\;%PATH%

cd Client
dotnet publish -c Release || goto error

cd ../ClientGUI
call VsDevCmd.bat
msbuild ClientGUI.csproj -p:Configuration=Release || goto error

cd ..

move Client\bin\Release\netcoreapp3.0\SysDVR-ClientGUI.exe Client\bin\Release\netcoreapp3.0\publish\SysDVR-ClientGUI.exe 

del Client\bin\Release\netcoreapp3.0\publish\NLog.xml
del Client\bin\Release\netcoreapp3.0\publish\Rtsp.pdb

7z a Client.7z .\Client\bin\Release\netcoreapp3.0\publish\*

:error
pause