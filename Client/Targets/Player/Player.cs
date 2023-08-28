using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static SDL2.SDL;
using FFmpeg.AutoGen;
using static FFmpeg.AutoGen.ffmpeg;
using System.Threading;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using SysDVR.Client.Windows;
using SysDVR.Client.Core;
using SysDVR.Client.GUI;
using ImGuiNET;

namespace SysDVR.Client.Targets.Player
{
    struct SDLContext
    {
        public bool Initialized;
        public IntPtr Texture { get; init; }
        public SDL_Rect TextureSize;

        public object TextureLock { get; init; }
    }

    struct SDLAudioContext
    {
        public uint DeviceID { get; init; }
        public SDL_AudioCallback CallbackDelegate { get; init; }
        public GCHandle TargetHandle { get; init; }
    }

    unsafe class DecoderContext
    {
        public AVCodecContext* CodecCtx { get; init; }

        public AVFrame* RenderFrame;
        public AVFrame* ReceiveFrame;

        public AVFrame* Frame1 { get; init; }
        public AVFrame* Frame2 { get; init; }

        public object CodecLock { get; init; }

        public StreamSynchronizationHelper SyncHelper;
        public AutoResetEvent OnFrameEvent;
    }

    unsafe struct FormatConverterContext
    {
        public SwsContext* Converter { get; init; }
        public AVFrame* Frame { get; init; }
    }

    class PlayerManager : BaseStreamManager
    {
        public new H264StreamTarget VideoTarget;
        public new AudioStreamTarget AudioTarget;

        public PlayerManager(bool HasVideo, bool HasAudio, CancellationTokenSource cancel) : base(
            HasVideo ? new H264StreamTarget() : null,
            HasAudio ? new AudioStreamTarget() : null,
            cancel)
        {
            VideoTarget = base.VideoTarget as H264StreamTarget;
            AudioTarget = base.AudioTarget as AudioStreamTarget;
        }
    }
}
