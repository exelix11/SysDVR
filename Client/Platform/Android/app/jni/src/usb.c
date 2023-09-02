#include "JniHelper.h"
// dotnet will copy this to its own string once the call returns so it can safely be used as a scratch buffer
jchar tmpStringBuffer[0x100];

static void JavaCopyWstr(JNIEnv *env, jstring str)
{
    JavaStrCopyTo(env, str, tmpStringBuffer, sizeof(tmpStringBuffer));
}

static jclass usb = NULL;

void UsbInit()
{
    // cache the usb class so other threads can use it
    JNIEnv* env = GetJNIEnv();
    usb = (*env)->FindClass(env, "exelix11/sysdvr/DvrUsbHelper");
    usb = (*env)->NewGlobalRef(env, usb);
}

bool UsbAcquireSnapshot(int vid, int pid, int* deviceCount)
{
    DECLARE_JNI;
    jmethodID mid = STATIC_METHOD(usb, "SnapshotDevices", "(II)I");
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
    DECLARE_JNI;
    jmethodID mid = STATIC_METHOD(usb,"FreeCurrentSnapshot", "()V");
    (*env)->CallStaticVoidMethod(env, usb, mid);
}

jchar* UsbGetSnapshotDeviceSerial(int idx)
{
    DECLARE_JNI;
    jmethodID mid = STATIC_METHOD(usb,"GetSerialById", "(I)Ljava/lang/String;");
    jstring str = (jstring)(*env)->CallStaticObjectMethod(env, usb, mid, idx);
    JavaCopyWstr(env, str);
    (*env)->DeleteLocalRef(env, str);
    return tmpStringBuffer;
}

jchar* UsbGetLastError()
{
    DECLARE_JNI;
    jmethodID mid = STATIC_METHOD(usb,"GetLatError", "()Ljava/lang/String;");
    jstring str = (jstring)(*env)->CallStaticObjectMethod(env, usb, mid);
    JavaCopyWstr(env, str);
    (*env)->DeleteLocalRef(env, str);
    return tmpStringBuffer;
}

bool UsbOpenHandle(jchar* serial, void** handle)
{
    DECLARE_JNI;
    jmethodID mid = STATIC_METHOD(usb,"OpenBySerial", "(Ljava/lang/String;)I");
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
    DECLARE_JNI;
    jmethodID mid = STATIC_METHOD(usb, "CloseDevice", "(I)V");
    (*env)->CallStaticVoidMethod(env, usb, mid, handle);
}