#!/bin/sh

set -e

if [ ! -d "shared-modules" ]; then
	git clone --depth 1 https://github.com/flathub/shared-modules.git
fi

flatpak-builder --user --install --install-deps-from=flathub tmp com.github.exelix11.sysdvr.json --force-clean
flatpak build-bundle ~/.local/share/flatpak/repo SysDVR-Client.flatpak com.github.exelix11.sysdvr --runtime-repo=https://flathub.org/repo/flathub.flatpakrepo
