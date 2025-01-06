#!/bin/sh

set -e

if [ ! -d "shared-modules" ]; then
	git clone --depth 1 https://github.com/flathub/shared-modules.git
fi

cd .. # Go to Platforms folder

echo Checking dependencies...

mkdir -p Resources/linux-x64/native

# cimgui_rX is the git tag for for the release we are usign of cimgui
if [ ! -e "Resources/linux-x64/native/cimgui_r2" ]; then
	echo Downloading Cimgui...
	curl -L https://github.com/exelix11/CimguiSDL2Cross/releases/download/r2/linux-x64.zip -o linux-x64.zip
	unzip linux-x64.zip
	mv cimgui.so Resources/linux-x64/native/cimgui.so
	echo ok > "Resources/linux-x64/native/cimgui_r2"
fi

cd .. # Go to client root
dotnet publish -c Release -r linux-x64
cd Platform/Linux

if [ -d "dvr-build" ]; then
	rm -rf dvr-build
fi

mkdir -p dvr-build
cp -r ../../bin/Release/net9.0/linux-x64/publish/* dvr-build/

flatpak-builder --user --install --install-deps-from=flathub tmp com.github.exelix11.sysdvr.json --force-clean
flatpak build-bundle ~/.local/share/flatpak/repo SysDVR-Client.flatpak com.github.exelix11.sysdvr --runtime-repo=https://flathub.org/repo/flathub.flatpakrepo
