//#define DEBUG_COLLISIONS

using SysDVR.Client.Core;
using System;
using System.Collections.Generic;

namespace SysDVR.Client.Sources
{
	internal class PacketReplayTable : IDisposable
	{
		const int TableSize = 64;
		readonly PoolBuffer[] Table = new PoolBuffer[TableSize];

#if DEBUG_COLLISIONS
        record Measure(int inserts, DateTime time);
        Dictionary<int, Measure> Inserts = new();
        Dictionary<int, int> Collisions = new();
        int inserts;

        void DebugInsert(int slot) 
        {
			++inserts;

            if (Table[slot] is not null)
            {
				var m = Inserts[slot];
				var n = new Measure(inserts, DateTime.Now);
				Inserts[slot] = n;
				++Collisions[slot];

				Console.WriteLine($"Slot collided {slot:x} collided after {n.inserts - m.inserts} inserts ({(n.time - m.time).TotalSeconds} seconds)");
			}
            else
            {
				Collisions.Add(slot, 0);
				Inserts.Add(slot, new Measure(inserts, DateTime.Now));
			}
		}
#endif

		public void AssignSlot(PoolBuffer block, int slot)
		{
#if DEBUG_COLLISIONS
			DebugInsert(slot);
#endif

			if (Table[slot] is not null)
			{
				Table[slot].Free();
			}

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
