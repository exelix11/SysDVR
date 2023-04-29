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
        ulong LastAudioTs;

        // Timestamps are in microseconds
        const ulong BaseMinDifference = 90 * 1000;
        const ulong MinDifferenceIncrement = 4 * 1000;

        // Some games stress the network protocol more than others so we start with a baseline 
        // difference of 100ms and every time a synchronization error happens we increase the
        // threshold by 4ms, even though at that point the delay may be noticeable.
        ulong AudioThreshold = BaseMinDifference;

        // Video has less buffering so it can have a smaller threshold
        // Also it can block when the UI thread is busy so we only use this
        // threshold to drop packets when they're too late cause UI was busy
        // Compensate any actual delay with the audio threshold
        const ulong VideoThreshold = BaseMinDifference;

        const ulong MaxAudioDifferenceUs = 200 * 1000;
        
        void AudioIncrementDelay()
        {
            if (AudioThreshold < MaxAudioDifferenceUs)
                AudioThreshold += MinDifferenceIncrement;
        }

        // Updates the timestamps and drops packets that are behind
        public bool CheckTimestamp(bool isVideo, ulong now)
        {
            if (!Enabled)
                return true;

            if (isVideo)
            {
                LastVideoTs = now;

                var audio = LastAudioTs;
                if (audio > now && audio - now > VideoThreshold)
                {
                    return false;
                }
            }
            else
            {
                LastAudioTs = now;

                var video = LastVideoTs;
                if (video > now && video - now > AudioThreshold)
                {
                    AudioIncrementDelay();
                    return false;
                }
            }

            return true;
        }
    }
}
