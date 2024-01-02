#include <stdbool.h>
#include <jni.h>
#include <android/log.h>

#define L(...) __android_log_print(ANDROID_LOG_ERROR, "SysDVRLogger", __VA_ARGS__)

// Exported from SDL, requires my patch to the source
JavaVM *AndroidGetJavaVM();
JNIEnv *Android_JNI_GetEnv_NoAttach(void);
JNIEnv *Android_JNI_GetEnv(void);

static _Thread_local JNIEnv *localEnv = NULL;

JNIEnv* GetJNIEnv()
{
    if (!localEnv)
    {
        JNIEnv* env = Android_JNI_GetEnv_NoAttach();
        if (env) // This is an SDL thread
        {
            return env;
        }

        L("BUG: Called GetJNIEnv from a non-attached thread !!!");
    }

    return localEnv;
}

void AttachThread()
{
    if (localEnv || Android_JNI_GetEnv_NoAttach())
    {
        L("BUG: This thread is already attached");
        return;
    }

    L("Attaching thread");

    JNIEnv* env = NULL;
    JavaVM* mJavaVM = AndroidGetJavaVM();
    if (!mJavaVM)
    {
        L("Failed to get JavaVM");
    }

    JavaVMAttachArgs vmAttachArgs;
    vmAttachArgs.version = JNI_VERSION_1_6;
    vmAttachArgs.name = NULL;
    vmAttachArgs.group = NULL;
    if ((*mJavaVM)->AttachCurrentThread(mJavaVM, &env, &vmAttachArgs) == JNI_OK)
        localEnv = env;
    else
        L("Failed to attach thread");
}

void DetachThread()
{
    if (!localEnv)
    {
        L("BUG: This thread is not attached");
        return;
    }

    L("Detaching thread");

    JavaVM* mJavaVM = AndroidGetJavaVM();
    if (!mJavaVM)
        L("Failed to get JavaVM");

    (*mJavaVM)->DetachCurrentThread(mJavaVM);
    localEnv = NULL;
}

// The main thread is managed by SDL so it must be attached differently
void InitThreading()
{
    L("Initializing threading");
    Android_JNI_GetEnv();
}