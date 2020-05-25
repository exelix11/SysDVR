#pragma once

#define SYSDVR_CRASH_MODULEID 0x69

#define ERR_RTSP_VIDEO MAKERESULT(SYSDVR_CRASH_MODULEID, 1)
#define ERR_RTSP_AUDIO MAKERESULT(SYSDVR_CRASH_MODULEID, 2)
#define ERR_TCP_VIDEO MAKERESULT(SYSDVR_CRASH_MODULEID, 3)
#define ERR_TCP_AUDIO MAKERESULT(SYSDVR_CRASH_MODULEID, 4)
#define ERR_USB_VIDEO MAKERESULT(SYSDVR_CRASH_MODULEID, 5)
#define ERR_USB_AUDIO MAKERESULT(SYSDVR_CRASH_MODULEID, 6)
#define ERR_RTSP_MULTI_NAL MAKERESULT(SYSDVR_CRASH_MODULEID, 7)
#define ERR_AUDIO_BATCH_SIZE MAKERESULT(SYSDVR_CRASH_MODULEID, 8)

/*
Debugging socket crash:
	Error module: 0xAn where n depends on the caller of CreateTCPListener
	the description is the check that failed in AttemptOpenTCPListener
*/
#define ERR_SOCK_FAIL(flag, error) MAKERESULT(0xA0 | flag, error)
#define ERR_SOCK_RTSP_1 1
#define ERR_SOCK_RTSP_2 2
#define ERR_SOCK_TCP_1 3
#define ERR_SOCK_TCP_2 4
#define ERR_SOCK_CONFIG_1 5
#define ERR_SOCK_CONFIG_2 6

//This is a version for the SysDVR Config app protocol, it's not shown anywhere and not related to the major version
#define SYSDVR_VERSION 3
#define TYPE_MODE_USB 1
#define TYPE_MODE_TCP 2
#define TYPE_MODE_RTSP 4
#define TYPE_MODE_NULL 3
#define TYPE_MODE_ERROR 999999