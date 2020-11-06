@echo off

set PATH=C:\Program Files\7zip;C:\Program Files\7-Zip\;C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\Tools\;%PATH%

cd Client
dotnet publish -c Release || goto error

cd ../ClientGUI
call VsDevCmd.bat
msbuild ClientGUI.csproj -p:Configuration=Release || goto error

cd ..

move Client\bin\Release\netcoreapp3.1\SysDVR-ClientGUI.exe Client\bin\Release\netcoreapp3.1\publish\SysDVR-ClientGUI.exe 

del Client\bin\Release\netcoreapp3.1\publish\NLog.xml
del Client\bin\Release\netcoreapp3.1\publish\Rtsp.pdb

7z a Client.7z .\Client\bin\Release\netcoreapp3.1\publish\*

:error
pause