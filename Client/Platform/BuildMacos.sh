#!/bin/sh

set -e

cd .. # Go to client root

APP_NAME="SysDVR Client"
EXECUTABLE_NAME="SysDVR-Client"
BUNDLE_ID="com.exelix.sysdvr.client"
APP_VERSION=$(cat release_version.txt)

extract_dmg() {
	dmg_file="$1"
	tmp_dir="$2"

	mkdir -p "$tmp_dir/mnt"
	hdiutil attach "$dmg_file" -mountpoint "$tmp_dir/mnt" -nobrowse -quiet
	cp -R "$tmp_dir/mnt/." "$tmp_dir/"
	hdiutil detach "$tmp_dir/mnt" -quiet
	rm -rf "$tmp_dir/mnt"
}

create_icns_icon() {
	out_path="$1"
	iconset_dir="tmp/Icon.iconset"
	rm -rf "$iconset_dir"
	mkdir -p "$iconset_dir"

	# Convert existing Windows icon into all sizes required for a macOS .icns.
	sips -s format png Client.ico --out tmp/source-icon.png >/dev/null 2>&1
	sips -z 16 16 tmp/source-icon.png --out "$iconset_dir/icon_16x16.png" >/dev/null 2>&1
	sips -z 32 32 tmp/source-icon.png --out "$iconset_dir/icon_16x16@2x.png" >/dev/null 2>&1
	sips -z 32 32 tmp/source-icon.png --out "$iconset_dir/icon_32x32.png" >/dev/null 2>&1
	sips -z 64 64 tmp/source-icon.png --out "$iconset_dir/icon_32x32@2x.png" >/dev/null 2>&1
	sips -z 128 128 tmp/source-icon.png --out "$iconset_dir/icon_128x128.png" >/dev/null 2>&1
	sips -z 256 256 tmp/source-icon.png --out "$iconset_dir/icon_128x128@2x.png" >/dev/null 2>&1
	sips -z 256 256 tmp/source-icon.png --out "$iconset_dir/icon_256x256.png" >/dev/null 2>&1
	sips -z 512 512 tmp/source-icon.png --out "$iconset_dir/icon_256x256@2x.png" >/dev/null 2>&1
	sips -z 512 512 tmp/source-icon.png --out "$iconset_dir/icon_512x512.png" >/dev/null 2>&1
	sips -z 1024 1024 tmp/source-icon.png --out "$iconset_dir/icon_512x512@2x.png" >/dev/null 2>&1
	iconutil -c icns "$iconset_dir" -o "$out_path"
}

create_macos_app_bundle() {
	build_root="$1"
	app_target="$2"
	runtime="$3"

	app_contents="$app_target/Contents"
	app_macos="$app_contents/MacOS"
	app_resources="$app_contents/Resources"

	rm -rf "$app_target"
	mkdir -p "$app_macos"
	mkdir -p "$app_resources"

	cp -R "$build_root/." "$app_macos/"
	chmod +x "$app_macos/$EXECUTABLE_NAME"
	create_icns_icon "$app_resources/AppIcon.icns"

	cat > "$app_contents/Info.plist" <<EOF
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
	<key>CFBundleDevelopmentRegion</key>
	<string>en</string>
	<key>CFBundleDisplayName</key>
	<string>$APP_NAME</string>
	<key>CFBundleExecutable</key>
	<string>$EXECUTABLE_NAME</string>
	<key>CFBundleIconFile</key>
	<string>AppIcon</string>
	<key>CFBundleIdentifier</key>
	<string>$BUNDLE_ID.$runtime</string>
	<key>CFBundleInfoDictionaryVersion</key>
	<string>6.0</string>
	<key>CFBundleName</key>
	<string>$APP_NAME</string>
	<key>CFBundlePackageType</key>
	<string>APPL</string>
	<key>CFBundleShortVersionString</key>
	<string>$APP_VERSION</string>
	<key>CFBundleVersion</key>
	<string>$APP_VERSION</string>
	<key>LSMinimumSystemVersion</key>
	<string>11.0</string>
	<key>NSHighResolutionCapable</key>
	<true/>
</dict>
</plist>
EOF

	# Ad-hoc sign app bundle so Gatekeeper can inspect a coherent signature.
	codesign --deep --force --sign - "$app_target"
}

echo Checking dependencies...

# All dependencies for macos are "fat" binaries meaning they contain both x64 and arm64 code
# so we can just download them once and use them for both architectures
# To avoid duplicate files we manually copy them in the release zip instead of puttig them in the runtimes folder
mkdir -p macos-deps
mkdir -p tmp

# cimgui_rX is the git tag for for the release we are usign of cimgui
if [ ! -e "macos-deps/cimgui_r2" ]; then
	echo Downloading Cimgui...
	curl -L https://github.com/exelix11/CimguiSDL2Cross/releases/download/r2/macos-fat-binaries.zip -o macos-fat-binaries.zip
	7z x macos-fat-binaries.zip -Omacos-deps
	rm macos-fat-binaries.zip
	rm -rf macos-deps/Resources
	echo ok > "macos-deps/cimgui_r2"
fi

if [ ! -e "macos-deps/SDL2_image.dylib" ]; then
	echo Downloading SDL2-image...
	curl -L https://github.com/libsdl-org/SDL_image/releases/download/release-2.6.3/SDL2_image-2.6.3.dmg -o SDL2_image.dmg
	extract_dmg SDL2_image.dmg tmp
	rm -f SDL2_image.dmg
	mv tmp/SDL2_image.framework/Versions/A/SDL2_image macos-deps/SDL2_image.dylib
	rm -rf tmp/SDL2_image.framework
fi

if [ ! -e "macos-deps/libavcodec.dylib" ]; then
	echo Downloading ffmpeg...
	curl -L https://github.com/exelix11/ffmpeg-kit/releases/download/v5.1/ffmpeg-kit-full-gpl-5.1-macos-xcframework.zip -o ffmpeg.zip
	7z x ffmpeg.zip -Otmp
	rm ffmpeg.zip

	mv tmp/libavcodec.xcframework/macos-arm64_x86_64/libavcodec.framework/Versions/A/libavcodec macos-deps/libavcodec.dylib
	mv tmp/libavdevice.xcframework/macos-arm64_x86_64/libavdevice.framework/Versions/A/libavdevice macos-deps/libavdevice.dylib
	mv tmp/libavfilter.xcframework/macos-arm64_x86_64/libavfilter.framework/Versions/A/libavfilter macos-deps/libavfilter.dylib
	mv tmp/libavformat.xcframework/macos-arm64_x86_64/libavformat.framework/Versions/A/libavformat macos-deps/libavformat.dylib
	mv tmp/libavutil.xcframework/macos-arm64_x86_64/libavutil.framework/Versions/A/libavutil macos-deps/libavutil.dylib
	mv tmp/libswresample.xcframework/macos-arm64_x86_64/libswresample.framework/Versions/A/libswresample macos-deps/libswresample.dylib	
	mv tmp/libswscale.xcframework/macos-arm64_x86_64/libswscale.framework/Versions/A/libswscale macos-deps/libswscale.dylib	

	rm -rf tmp/*
fi

if [ ! -e "macos-deps/libusb-1.0.dylib" ]; then
	echo Downloading libusb
	# Note the name difference 1.0.0 -> 1.0
	curl -L https://github.com/exelix11/libusb-builds/releases/download/v0/libusb-1.0.0.dylib -o macos-deps/libusb-1.0.dylib
fi

# Stip all dylibs signatures, this is a complicated mess to figure out and i don't have a mac
# so we're doing it the windows way(tm) with no signatures at all
codesign --remove-signature -v macos-deps/*.dylib

# For arm macs we need to use at least ad-hoc signed libraries or else they won't load at all...
codesign -s - -v macos-deps/*.dylib

echo Building x64 client...
dotnet publish -c Release -r osx-x64 /p:SysDvrTarget=macos

mkdir -p MacOsBuild-x64
cp -r bin/Release/net9.0/osx-x64/publish/* MacOsBuild-x64/
mkdir -p MacOsBuild-x64/runtimes/osx-x64/native
cp macos-deps/* MacOsBuild-x64/runtimes/osx-x64/native/
create_macos_app_bundle "MacOsBuild-x64" "$APP_NAME-x64.app" "osx-x64"
cd MacOsBuild-x64
zip -r "../SysDVRClient-MacOs-x64.zip" *
cd ..
zip -r "SysDVRClient-MacOs-x64-App.zip" "$APP_NAME-x64.app"

echo Building arm64 client...
dotnet publish -c Release -r osx-arm64 /p:SysDvrTarget=macos

mkdir -p MacOsBuild-arm64
cp -r bin/Release/net9.0/osx-arm64/publish/* MacOsBuild-arm64/
mkdir -p MacOsBuild-arm64/runtimes/osx-arm64/native
cp macos-deps/* MacOsBuild-arm64/runtimes/osx-arm64/native/
create_macos_app_bundle "MacOsBuild-arm64" "$APP_NAME-arm64.app" "osx-arm64"
cd MacOsBuild-arm64
zip -r "../SysDVRClient-MacOs-arm64.zip" *
cd ..
zip -r "SysDVRClient-MacOs-arm64-App.zip" "$APP_NAME-arm64.app"

echo Done !
