#include <stdbool.h>
#include <jni.h>
#include <android/log.h>

#define L(...) __android_log_print(ANDROID_LOG_ERROR, "SysDVRLogger", __VA_ARGS__)

// From Thread.c
JNIEnv* GetJNIEnv();

void LOG(const char* string);

// dotnet will copy this to its own string once the call returns so it can safely be used a sa scratch buffer
jchar tmpStringBuffer[0x100];

static int min(int a, int b)
{
    return a > b ? b : a;
}

jint jstrlen(jchar* str)
{
    jint len = 0;
    while (*str)
    {
        ++len;
        ++str;
    }

    return len;
}

static void JavaCopyWstr(JNIEnv *env, jstring str)
{
    const jchar *raw = (*env)->GetStringChars(env, str, 0);
    jsize len = (*env)->GetStringLength(env, str);
    int i = 0;

    for (i = 0; i < min(len, sizeof(tmpStringBuffer) - 1); i++)
        tmpStringBuffer[i] = raw[i];

    tmpStringBuffer[i] = '\0';
    (*env)->ReleaseStringChars(env, str, raw);
}

static jclass usb = NULL;

#define METHOD(name, signature) \
    JNIEnv* env = GetJNIEnv();  \
    L("Calling JNIEnv from %s %p", __FUNCTION__, env); \
    jmethodID mid = (*env)->GetStaticMethodID(env, usb, name, signature);

void UsbInit()
{
    // cache the usb class so other threads can use it
    JNIEnv* env = GetJNIEnv();
    usb = (*env)->FindClass(env, "exelix11/sysdvr/DvrUsbHelper");
    usb = (*env)->NewGlobalRef(env, usb);
}

bool UsbAcquireSnapshot(int vid, int pid, int* deviceCount)
{
    METHOD("SnapshotDevices", "(II)I");
    jint result = (*env)->CallStaticIntMethod(env, usb, mid, vid, pid);
    if (result == -1)
    {
        *deviceCount = 0;
        return false;
    }
    *deviceCount = result;
    return true;
}

void UsbReleaseSnapshot()
{
    METHOD("FreeCurrentSnapshot", "()V");
    (*env)->CallStaticVoidMethod(env, usb, mid);
}

jchar* UsbGetSnapshotDeviceSerial(int idx)
{
    METHOD("GetSerialById", "(I)Ljava/lang/String;");
    jstring str = (jstring)(*env)->CallStaticObjectMethod(env, usb, mid, idx);
    JavaCopyWstr(env, str);
    (*env)->DeleteLocalRef(env, str);
    return tmpStringBuffer;
}

jchar* UsbGetLastError()
{
    METHOD("GetLatError", "()Ljava/lang/String;");
    jstring str = (jstring)(*env)->CallStaticObjectMethod(env, usb, mid);
    JavaCopyWstr(env, str);
    (*env)->DeleteLocalRef(env, str);
    return tmpStringBuffer;
}

bool UsbOpenHandle(jchar* serial, void** handle)
{
    METHOD("OpenBySerial", "(Ljava/lang/String;)I");
    jstring str = (*env)->NewString(env, serial, jstrlen(serial));
    jint res = (*env)->CallStaticIntMethod(env, usb, mid, str);
    (*env)->DeleteLocalRef(env, str);
    if (res == 0)
    {
        *handle = NULL;
        return false;
    }
    *handle = (void*)res;
    return true;
}

void UsbCloseHandle(void* handle)
{
    METHOD("CloseDevice", "(I)V");
    (*env)->CallStaticVoidMethod(env, usb, mid, handle);
}