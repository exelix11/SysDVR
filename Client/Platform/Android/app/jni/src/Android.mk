LOCAL_PATH := $(call my-dir)

include $(CLEAR_VARS)

LOCAL_MODULE := main

SDL_PATH := ../SDL

LOCAL_C_INCLUDES := $(LOCAL_PATH)/$(SDL_PATH)/include

LOCAL_SRC_FILES := main.c usb.c thread.c

LOCAL_SHARED_LIBRARIES := SDL2 SDL2_image cimgui Client-prebuilt libusb1.0

LOCAL_LDLIBS := -lGLESv1_CM -lGLESv2 -lOpenSLES -llog -landroid -ldl

include $(BUILD_SHARED_LIBRARY)