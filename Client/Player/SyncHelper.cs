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

        // Timestamps are in microseconds
        const ulong BaseMinDifference = 90 * 1000;
        const ulong MinDifferenceIncrement = 10 * 1000;

        // Some games stress the network protocol more than others so we start with a baseline 
        // difference of 100ms and every time a synchronization error happens we increase the
        // threshold by 10ms, even though at that point the delay may be noticeable.
        ulong AudioThreshold = BaseMinDifference;

        const ulong MaxAudioDifferenceUs = 240 * 1000;

        void AudioIncrementDelay() 
        {
            if (AudioThreshold < MaxAudioDifferenceUs)
                AudioThreshold += MinDifferenceIncrement;
        }

        // Updates the timestamps and drops packets that are behind
        // With this implementation we make audio always follow video, this is because
        // video has lower buffering and it's less likely to have a delay.
        // Also video can freeze if the UI thread becomes unresponsivle
        // (click on cmd, dragging the window SDL bug and so on) so we can't do the other way.
        public bool CheckTimestamp(bool isVideo, ulong now)
        {
            if (!Enabled)
                return true;

            if (isVideo)
            {
                LastVideoTs = now;
            }
            else
            {
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
