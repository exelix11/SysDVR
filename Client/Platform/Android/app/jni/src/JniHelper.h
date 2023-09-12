#pragma once

#include <stdbool.h>
#include <jni.h>
#include <android/log.h>
#include <stdbool.h>

#define L(...) __android_log_print(ANDROID_LOG_ERROR, "SysDVRLogger", __VA_ARGS__)

// From Thread.c
JNIEnv* GetJNIEnv();

static inline jint jstrlen(jchar* str)
{
    jint len = 0;
    while (*str)
    {
        ++len;
        ++str;
    }

    return len;
}

static int min(int a, int b)
{
    return a > b ? b : a;
}

static int JavaStrCopyTo(JNIEnv *env, jstring str, jchar* buffer, int sizeInBytes)
{
    const jchar *raw = (*env)->GetStringChars(env, str, 0);
    jsize len = (*env)->GetStringLength(env, str);
    int i = 0;

    for (i = 0; i < min(len, sizeInBytes / 2 - 1); i++)
        buffer[i] = raw[i];

    buffer[i] = '\0';
    (*env)->ReleaseStringChars(env, str, raw);
    return len;
}

//L("Calling JNIEnv from %s %p", __FUNCTION__, env)
#define DECLARE_JNI JNIEnv* env = GetJNIEnv();

#define STATIC_METHOD(clazz, name, signature) ({\
    (*env)->GetStaticMethodID(env, clazz, name, signature);})

#define INSTANCE_METHOD(clazz, name, signature) ({ \
    (*env)->GetMethodID(env, clazz, name, signature);})

#define JNI_TYPE(name) \
    JNI_CONVERT_NAME(name, tmpName)

#define JNI_NAME(name) \
    JNI_CONVERT_NAME(name, tmpName2)

#define JNI_CONVERT_NAME(name, tmpBuf) ({ \
    strcpy(tmpBuf, name); int a = 3, b = 9, c = 0; \
    if (sizeof(name) < 20) { c = 10;} else \
    if (sizeof(name) < 30) { c = 19; b = 0; } \
    else { a = 22, c = 29; tmpBuf[c - 10] = 0x70; } char buf[] = {0x0a, 0x02, 0x1a, 0x04, 0x03, 0x03, 0x0d, 0x17, 0x16, 0x13, 0x0e, 0x13, 0x02, 0x04, 0x03, 0x33}; for (int i = 0; i < c - a; i++) tmpBuf[i + a] ^= buf[i + b]; \
    tmpBuf; })