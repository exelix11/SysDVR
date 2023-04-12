#!/bin/sh

set -e

if [ ! -d "shared-modules" ]; then
	git clone --depth 1 https://github.com/flathub/shared-modules.git
fi

dotnet publish ../Client.csproj -c Release 

if [ -d "dvr-build" ]; then
	rm -rf dvr-build
fi

mkdir -p dvr-build
mv ../bin/Release/net6.0/publish/* dvr-build/

ARG=""
# workaround for GitHub Actions always running as root
if [ "$(id -u)" -eq 0 ]; then
	ARG=""
else
	ARG="--user"
fi

# install flatpak dependencies
flatpak install $ARG --noninteractive --assumeyes flathub org.freedesktop.Platform//22.08 org.freedesktop.Sdk//22.08 runtime/org.freedesktop.Sdk.Extension.dotnet6/x86_64/22.08

flatpak-builder $ARG --install tmp com.github.exelix11.sysdvr.json --force-clean
flatpak build-bundle ~/.local/share/flatpak/repo SysDVR-Client.flatpak com.github.exelix11.sysdvr --runtime-repo=https://flathub.org/repo/flathub.flatpakrepo