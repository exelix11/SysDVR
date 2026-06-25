#!/bin/sh

# Exit immediately if any command fails
set -e

# Go to client root
cd ..

echo "=== SysDVR Client iOS Build Script ==="
echo "Note: iOS does not support raw USB streaming natively without specialized profiles/jailbreak."
echo "The iOS build will default to Wi-Fi / Network streaming only."
echo ""

# Setup output folders
mkdir -p ios-build
mkdir -p ios-build/device
mkdir -p ios-build/simulator-arm64
mkdir -p ios-build/simulator-x64

echo "Building C# client library with NativeAOT..."

# 1. Build for physical iOS device (ios-arm64)
echo ""
echo "[1/3] Building for iOS Device (ios-arm64)..."
dotnet publish -c Release -r ios-arm64 /p:SysDvrTarget=ios

# Copy the compiled NativeAOT dynamic/static library to the build folder
cp -r bin/Release/net9.0/ios-arm64/publish/* ios-build/device/

# 2. Build for iOS Simulator on Apple Silicon (iossimulator-arm64)
echo ""
echo "[2/3] Building for iOS Simulator (iossimulator-arm64)..."
dotnet publish -c Release -r iossimulator-arm64 /p:SysDvrTarget=ios
cp -r bin/Release/net9.0/iossimulator-arm64/publish/* ios-build/simulator-arm64/

# 3. Build for iOS Simulator on Intel (iossimulator-x64)
echo ""
echo "[3/3] Building for iOS Simulator (iossimulator-x64)..."
dotnet publish -c Release -r iossimulator-x64 /p:SysDvrTarget=ios
cp -r bin/Release/net9.0/iossimulator-x64/publish/* ios-build/simulator-x64/

echo Done !
