using System;

namespace SysDVRClientGUI
{
    [Flags]
    public enum StreamKind : uint
    {
        Video = 1,
        Audio = 1 << 1,
        Both = Video | Audio
    }

    public enum StreamSource
    {
        Usb,
        Tcp
    }

    public enum StreamMode
    {
        Play,
        PlayMpv,
        Rtsp,
        SaveToFile
    }
}
