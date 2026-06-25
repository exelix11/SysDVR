#!/bin/bash

# Exit immediately if any command fails
set -e

# Ensure full Xcode toolchain is selected for xcrun / dotnet publish iOS compilation
export DEVELOPER_DIR="/Applications/Xcode.app/Contents/Developer"

# Get the script directory
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
CLIENT_DIR="$(cd "$SCRIPT_DIR/../.." && pwd)"

echo "=== SysDVR iOS IPA Packaging Automation ==="
echo "Script directory: $SCRIPT_DIR"
echo "Client directory: $CLIENT_DIR"
echo ""

# 1. Compile C# Library
echo "=== 1/5 Compiling C# Client Library ==="
cd "$CLIENT_DIR"
dotnet publish -c Release -r ios-arm64 /p:SysDvrTarget=ios

# Verify publish output exists
CS_DYLIB="$CLIENT_DIR/bin/Release/net9.0/ios-arm64/publish/SysDVR-Client.dylib"
if [ ! -f "$CS_DYLIB" ]; then
    echo "Error: C# compilation output not found at $CS_DYLIB"
    exit 1
fi

mkdir -p "$CLIENT_DIR/ios-build/device"
cp -f "$CS_DYLIB" "$CLIENT_DIR/ios-build/device/SysDVR-Client.dylib"
echo "C# library published to ios-build/device/SysDVR-Client.dylib"
echo ""

# 2. Build Xcode Project
echo "=== 2/5 Compiling Xcode Project ==="
export DEVELOPER_DIR="/Applications/Xcode.app/Contents/Developer"

# Clean local build directory to avoid cached artifacts
rm -rf "$SCRIPT_DIR/build"

# Run xcodebuild with local SYMROOT and OTHER_LDFLAGS
xcodebuild -project "$SCRIPT_DIR/sysDVR/sysDVR.xcodeproj" \
  -scheme sysDVR \
  -configuration Release \
  -destination 'generic/platform=iOS' \
  CODE_SIGNING_ALLOWED=NO \
  OTHER_LDFLAGS="$CLIENT_DIR/ios-build/device/SysDVR-Client.dylib" \
  SYMROOT="$SCRIPT_DIR/build"

echo "Xcode build completed successfully."
echo ""

# 3. Embed C# Dynamic Library
APP_PATH="$SCRIPT_DIR/build/Release-iphoneos/sysDVR.app"
if [ ! -d "$APP_PATH" ]; then
    echo "Error: sysDVR.app not found at $APP_PATH"
    exit 1
fi

echo "=== 3/5 Embedding Dynamic Libraries and Resources ==="
mkdir -p "$APP_PATH/Frameworks"
cp -f "$CLIENT_DIR/ios-build/device/SysDVR-Client.dylib" "$APP_PATH/Frameworks/SysDVR-Client.dylib"
cp -f "$SCRIPT_DIR/sysDVR/libcimgui.dylib" "$APP_PATH/Frameworks/libcimgui.dylib"
echo "Embedded SysDVR-Client.dylib and libcimgui.dylib in sysDVR.app/"
echo ""

# 4. Package Unsigned IPA
echo "=== 4/5 Packaging Unsigned IPA ==="
PKG_DIR="$SCRIPT_DIR/build/Packaging"
rm -rf "$PKG_DIR"
mkdir -p "$PKG_DIR/Payload"

# Copy sysDVR.app to Payload/
cp -R "$APP_PATH" "$PKG_DIR/Payload/"

# Zip Payload directory
cd "$PKG_DIR"
zip -r -q "$CLIENT_DIR/ios-build/sysDVR-unsigned.ipa" Payload

# Clean up packaging folder
cd "$SCRIPT_DIR"
rm -rf "$PKG_DIR"
echo ""

# 5. Done
echo "=== 5/5 Build Process Finished ==="
echo "Success! The unsigned IPA file has been packaged at:"
echo "  $CLIENT_DIR/ios-build/sysDVR-unsigned.ipa"
echo ""
