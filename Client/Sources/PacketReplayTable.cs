using SysDVR.Client.Core;
using System;

namespace SysDVR.Client.Sources
{
    internal class PacketReplayTable : IDisposable
    {
        const int TableSize = 20;

        readonly PoolBuffer[] Table = new PoolBuffer[TableSize];

        public void AssignSlot(PoolBuffer block, int slot)
        {
            if (Table[slot] is not null)
                Table[slot].Free();

            //Console.WriteLine($"Adding hash {hash:x} {Counter}");
            Table[slot] = block;
            block.Reference();
        }

        public void Flush()
        {
            for (int i = 0; i < TableSize; i++)
            {
                Table[i]?.Free();
                Table[i] = null;
            }
        }

        public bool LookupSlot(int slot, out PoolBuffer buffer)
        {
            // invaid slot
            if (slot == 0xFF)
            {
                buffer = default;
                return false;
            }

            buffer = Table[slot];
            buffer?.Reference();
            return buffer is not null;
        }

        public void Dispose()
        {
            Flush();
        }
    }
}
