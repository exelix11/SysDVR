LOCAL_PATH := $(call my-dir)

include $(CLEAR_VARS)

LOCAL_MODULE := cimgui

SDL_PATH := ../SDL

LOCAL_CFLAGS := \
	-DIMGUI_DISABLE_OBSOLETE_FUNCTIONS=1 \
	-DIMGUI_IMPL_API='extern "C" '

LOCAL_C_INCLUDES := \
	$(LOCAL_PATH)/$(SDL_PATH)/include \
	$(LOCAL_PATH)/cimgui/imgui

# Add your application source files here...
LOCAL_SRC_FILES := \
    cimgui/imgui/imgui.cpp \
	cimgui/imgui/imgui_tables.cpp \
    cimgui/imgui/imgui_draw.cpp \
    cimgui/imgui/imgui_demo.cpp \
    cimgui/imgui/imgui_widgets.cpp \
	cimgui/imgui/backends/imgui_impl_sdl2.cpp \
	cimgui/imgui/backends/imgui_impl_sdlrenderer2.cpp \
	cimgui/cimgui.cpp

LOCAL_SHARED_LIBRARIES := SDL2 SDL2_image

LOCAL_LDLIBS := -lGLESv1_CM -lGLESv2 -lOpenSLES -llog -landroid

include $(BUILD_SHARED_LIBRARY)
