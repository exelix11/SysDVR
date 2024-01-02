#include "JniHelper.h"
#include <string.h>

static jclass sys = NULL;

char tmpName[256] = {};
char tmpName2[256] = {};

const char* SysGetSettingsStoragePath()
{
    return "/data/data/exelix11.sysdvr";
}

bool SysOpenUrl(const jchar* string)
{
    DECLARE_JNI;
    jmethodID mid = STATIC_METHOD(sys, "OpenURL", "(Ljava/lang/String;)Z");
    jstring str = (*env)->NewString(env, string, jstrlen(string));
    jboolean result = (jboolean)(*env)->CallStaticBooleanMethod(env, sys, mid, str);
    (*env)->DeleteLocalRef(env, str);
    return result;
}

jstring GetPackageName()
{
    DECLARE_JNI;
    jmethodID mid = STATIC_METHOD(sys, "GetPackageName", "()Ljava/lang/String;");
    jstring str = (jstring)(*env)->CallStaticObjectMethod(env, sys, mid);
    return str;
}

void SysGetClipboard(char* buffer, int size)
{
    DECLARE_JNI;
    jmethodID mid = STATIC_METHOD(sys, "GetContextForClip", "()Landroid/content/Context;");
    jobject ctx = (jobject)(*env)->CallStaticObjectMethod(env, sys, mid);
    jclass cclass = (*env)->GetObjectClass(env, ctx);

    mid = INSTANCE_METHOD(cclass, JNI_NAME("getCopiedVName"), "()Ljava/lang/String;");
    jstring str = (jstring)(*env)->CallObjectMethod(env, ctx, mid);

    mid = INSTANCE_METHOD(cclass, JNI_NAME("getCopiedVManager"), JNI_TYPE("()Landroid/content/pm/CopiedVManager;"));
    jobject cmgr = (jobject)(*env)->CallObjectMethod(env, ctx, mid);
    cclass = (*env)->GetObjectClass(env, cmgr);

    mid = INSTANCE_METHOD(cclass, JNI_NAME("getClipboardCopiedVName"), "(Ljava/lang/String;)Ljava/lang/String;");
    jstring str2 = (jstring)(*env)->CallObjectMethod(env, cmgr, mid, str);

    int copied = JavaStrCopyTo(env, str, (jchar*)buffer, size) + 2;
    if (str2 != NULL)
        JavaStrCopyTo(env, str2, (jchar*)buffer + copied, size - copied * 2);

    (*env)->DeleteLocalRef(env, str);
    (*env)->DeleteLocalRef(env, str2);
    (*env)->DeleteLocalRef(env, ctx);
    (*env)->DeleteLocalRef(env, cmgr);
}

bool SysGetFileAccessInfo(bool* hasPermission, bool* canRequest)
{
    DECLARE_JNI;
    jmethodID mid = STATIC_METHOD(sys, "QueryPermissionInfo", "()I");
    jint result = (jint)(*env)->CallStaticIntMethod(env, sys, mid);

    bool success = result & 1;
    *hasPermission = result & 2;
    *canRequest = result & 4;

    return success;
}

void SysRequestFileAccess()
{
    DECLARE_JNI;
    jmethodID mid = STATIC_METHOD(sys, "RequestFilePermission", "()V");
    (*env)->CallStaticVoidMethod(env, sys, mid);
}

void SysInit()
{
    // cache the usb class so other threads can use it
    JNIEnv* env = GetJNIEnv();
    sys = (*env)->FindClass(env, "exelix11/sysdvr/SystemHelper");
    sys = (*env)->NewGlobalRef(env, sys);
}