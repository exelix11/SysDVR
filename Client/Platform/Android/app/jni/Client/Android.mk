LOCAL_PATH := $(call my-dir)

include $(CLEAR_VARS)

LOCAL_MODULE := Client-prebuilt

LOCAL_SRC_FILES := libClient.so

LOCAL_SHARED_LIBRARIES := SDL2 SDL2_image cimgui log

include $(PREBUILT_SHARED_LIBRARY)
