#import <UIKit/UIKit.h>
#include <SDL2/SDL.h>

struct NativeInitBlock
{
    void* Version;
    void* Sizeof;
    void* PrintFunction;

    void* AttachThread;
    void* DetachThread;

    void* UsbAcquireSnapshot;
    void* UsbReleaseSnapshot;
    void* UsbGetSnapshotDeviceSerial;
    void* UsbOpenHandle;
    void* UsbCloseHandle;
    void* UsbGetLastError;

    void* SysOpenURL;
    void* SysGetDynamicLibInfo;
    void* SysGetFileAccessInfo;
    void* SysRequestFileAccess;
    void* SysGetSettingsStoragePath;
    void* SysIterateAssetsContent;
    void* SysReadAssetFile;
    void* SysFreeDynamicBuffer;
};

void LOG(const char* string)
{
    NSLog(@"[SysDVR] %s", string);
}

void DummyFunc() {}

bool SysOpenUrl(const char* url)
{
    NSString *urlString = [NSString stringWithUTF8String:url];
    NSURL *u = [NSURL URLWithString:urlString];
    dispatch_async(dispatch_get_main_queue(), ^{
        if ([[UIApplication sharedApplication] canOpenURL:u]) {
            [[UIApplication sharedApplication] openURL:u options:@{} completionHandler:nil];
        }
    });
    return true;
}

const char* SysGetSettingsStoragePath()
{
    NSArray *paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
    NSString *documentsDirectory = [paths firstObject];
    return [documentsDirectory UTF8String];
}

struct NativeInitBlock g_native =
{
    .Version = (void*)1,
    .Sizeof = (void*)sizeof(struct NativeInitBlock),
    .PrintFunction = LOG,

    .AttachThread = DummyFunc,
    .DetachThread = DummyFunc,

    .UsbAcquireSnapshot = NULL,
    .UsbReleaseSnapshot = NULL,
    .UsbGetSnapshotDeviceSerial = NULL,
    .UsbOpenHandle = NULL,
    .UsbCloseHandle = NULL,
    .UsbGetLastError = NULL,

    .SysOpenURL = SysOpenUrl,
    .SysGetDynamicLibInfo = NULL,
    .SysGetFileAccessInfo = NULL,
    .SysRequestFileAccess = NULL,
    .SysGetSettingsStoragePath = SysGetSettingsStoragePath,
    .SysIterateAssetsContent = NULL,
    .SysReadAssetFile = NULL,
    .SysFreeDynamicBuffer = NULL
};

extern int sysdvr_entrypoint(struct NativeInitBlock* init);

int main(int argc, char *argv[])
{
    LOG("iOS SDL_main wrapper called");
    int result = sysdvr_entrypoint(&g_native);
    return result;
}

#ifdef main
#undef main
#endif

extern int SDL_UIKitRunApp(int argc, char *argv[], int (*mainFunction)(int, char*[]));

int main(int argc, char *argv[])
{
    return SDL_UIKitRunApp(argc, argv, SDL_main);
}
