#include "JniHelper.h"
#include <string.h>
#include <malloc.h>

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

void SysIterateAssetsContent(const wchar_t* path, bool(*callback)(const wchar_t*, int32_t))
{
    DECLARE_JNI;
    jmethodID mid = STATIC_METHOD(sys, "ListAssetsContent", "(Ljava/lang/String;)[Ljava/lang/String;");
    jstring pathstr = (*env)->NewString(env, path, jstrlen(path));
    jarray arr = (jarray)(*env)->CallStaticObjectMethod(env, sys, mid, pathstr);
    jsize len = (*env)->GetArrayLength(env, arr);
    for (jsize i = 0; i < len; i++)
    {
        jstring str = (*env)->GetObjectArrayElement(env, (jobjectArray)arr, i);
        jchar* chars = (*env)->GetStringChars(env, str, NULL);
        jsize len = (*env)->GetStringLength(env, str);
        bool should_continue = callback(chars, len);
        (*env)->ReleaseStringChars(env, str, chars);

        if (!should_continue)
            break;
    }
    (*env)->DeleteLocalRef(env, pathstr);
}

enum {
    ASSET_READ_OK = 0,
    ASSET_READ_ERROR = 1,
    ASSET_READ_ERROR_JNI = 2,
    ASSET_READ_ERROR_MALLOC = 3,
};

int SysReadAssetFile(const wchar_t* path, uint8_t ** buffer, int32_t* length)
{
    DECLARE_JNI;

    *length = 0;
    *buffer = NULL;

    jstring pathstr = (*env)->NewString(env, path, jstrlen(path));
    jmethodID mid = STATIC_METHOD(sys, "ReadAsset", "(Ljava/lang/String;)[B");
    jbyteArray data = (jbyteArray)(*env)->CallStaticObjectMethod(env, sys, mid, pathstr);

    (*env)->DeleteLocalRef(env, pathstr);

    if (!data)
        return ASSET_READ_ERROR;

    int result = ASSET_READ_OK;
    jsize jlen = (*env)->GetArrayLength(env, data);
    jbyte* jbuffer = (*env)->GetByteArrayElements(env, data, NULL);

    if (!jbuffer)
        result = ASSET_READ_ERROR_JNI;
    else
    {
        *buffer = malloc(jlen);
        if (!*buffer)
            result = ASSET_READ_ERROR_MALLOC;
        else
        {
            memcpy(*buffer, jbuffer, jlen);
            *length = jlen;
        }
        (*env)->ReleaseByteArrayElements(env, data, jbuffer, 0);
    }

    (*env)->DeleteLocalRef(env, data);
    return result;
}

void SysFreeDynamicBuffer(void* buffer)
{
    if (buffer)
        free(buffer);
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