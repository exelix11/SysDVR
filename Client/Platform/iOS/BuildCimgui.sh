#!/bin/bash
set -e
export DEVELOPER_DIR="/Applications/Xcode.app/Contents/Developer"
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
CIMGUI_SRC="/Users/hdrifi/SysDVR/CimguiSDL2Cross/cimgui"

echo "=== Compiling Cimgui for iOS ==="
cd "$CIMGUI_SRC"
clang++ -shared -dynamiclib -o "$SCRIPT_DIR/sysDVR/libcimgui.dylib" \
  -arch arm64 \
  -isysroot /Applications/Xcode.app/Contents/Developer/Platforms/iPhoneOS.platform/Developer/SDKs/iPhoneOS.sdk \
  -miphoneos-version-min=12.0 \
  -I"$SCRIPT_DIR/sysDVR/SDL2.framework/Headers" \
  -I"$CIMGUI_SRC" \
  -I"$CIMGUI_SRC/imgui" \
  -I"$CIMGUI_SRC/imgui/backends" \
  -F"$SCRIPT_DIR/sysDVR" \
  -framework SDL2 \
  -install_name @rpath/libcimgui.dylib \
  -DIMGUI_DISABLE_OBSOLETE_FUNCTIONS=1 \
  -DIMGUI_IMPL_API='extern "C"' \
  cimgui.cpp \
  imgui/imgui.cpp \
  imgui/imgui_draw.cpp \
  imgui/imgui_demo.cpp \
  imgui/imgui_widgets.cpp \
  imgui/imgui_tables.cpp \
  imgui/backends/imgui_impl_sdl2.cpp \
  imgui/backends/imgui_impl_sdlrenderer2.cpp

echo "Cimgui compiled successfully to sysDVR/libcimgui.dylib"
