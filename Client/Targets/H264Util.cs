using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysDVR.Client.Targets
{
    public static class H264Util
    {
        // Returns start and length of all NALs in the data block, excluding the start sequence 0 0 1
        public static IEnumerable<(int, int)> EnumerateNals(ArraySegment<byte> data)
        {
            int start = 0;
            for (int i = 2; i < data.Count; i++)
            {
                if (data[i - 2] == 0 && data[i - 1] == 0 && data[i] == 1)
                {
                    if (start != 0)
                    {
                        yield return (start + 1, i - start - 3);
                    }
                    start = i;
                }
            }
            if (start != 0)
            {
                yield return (start + 1, data.Count - start - 1);
            }
        }
    }
}
