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

        // Timestamps are in microseconds, 100ms
        const ulong MaxDifferenceUs = 100 * 1000;

        public void Reset()
        {
            LastAudiots = 0;
            LastVideoTs = 0;
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
                if (audio > now && audio - now > MaxDifferenceUs)
                    return false;
            }
            else
            {
                LastAudiots = now;

                var video = LastVideoTs;
                if (video > now && video - now > MaxDifferenceUs)
                    return false;
            }

            return true;
        }
    }
}
