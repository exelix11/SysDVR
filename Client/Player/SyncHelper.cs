using FFmpeg.AutoGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysDVR.Client.Player
{
    class StreamSynchronizationHelper
    {
        readonly bool Enabled;

        public StreamSynchronizationHelper(bool enable) 
        {
            if (!enable)
                Enabled = false;
            else 
                Enabled = !DebugOptions.Current.NoSync;
        }

        ulong LastVideoTs;
        ulong LastAudiots;

        // Timestamps are in microseconds
        const ulong BaseMinDifference = 90 * 1000;
        const ulong MinDifferenceIncrement = 100;

        // Some games stress the network protocol more than others so we start with a baseline 
        // difference of 100ms and every time a synchronization error happens we increase the
        // threshold by 10ms, even though at that point the delay may be noticeable.
        ulong VideoThreshold = BaseMinDifference;
        ulong AudioThreshold = BaseMinDifference;

        // Video has a lower threshold becuase it can freeze if the UI thread becomes unresponsivle
        // (click on cmd, dragging the window SDL bug and so on)
        const ulong MaxVideoDifferenceUs = 140 * 1000;
        const ulong MaxAudioDifferenceUs = 240 * 1000;

        void VideoIncrementDelay()
        {
            if (VideoThreshold < MaxVideoDifferenceUs)
                VideoThreshold += MinDifferenceIncrement;
        }

        void AudioIncrementDelay() 
        {
            if (AudioThreshold < MaxAudioDifferenceUs)
                AudioThreshold += MinDifferenceIncrement;

            Console.WriteLine($"Audio thres: {AudioThreshold}");
        }

        // Updates the timestamps and drops packets that are behind
        public bool CheckTimestamp(bool isVideo, ulong now)
        {
            if (!Enabled)
                return true;

            if (isVideo)
            {
                LastVideoTs = now;

                var audio = LastAudiots;
                // If audio is ahead of us of more than 200ms, drop the packet
                if (audio > now && audio - now > VideoThreshold)
                {
                    VideoIncrementDelay();
                    return false;
                }
            }
            else
            {
                LastAudiots = now;

                var video = LastVideoTs;
                if (video > now && video - now > AudioThreshold)
                {
                    AudioIncrementDelay();
                    return false;
                }

                Console.WriteLine($"{((long)video - (long)now) / 1000} {AudioThreshold / 1000}");
            }

            return true;
        }
    }
}
