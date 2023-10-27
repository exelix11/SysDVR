using Force.Crc32;
using SysDVR.Client.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace SysDVR.Client.Sources
{
    // This class implements the logic from CheckVideoPacket in capture.c
    // It is used to cache big video packets locally so they don't need to be transmitted
    // THe implementaiton on the client must be exactly equal to the one on the server or the hash tables will desync
    internal class PacketHashTable : IDisposable
    {
        const int HashTableSize = 20;

        struct TableEntry 
        {
            public uint Hash;
            public PoolBuffer Buffer;
        }

        private int Counter;
        readonly TableEntry[] Table = new TableEntry[HashTableSize];

        public void OnNewBlock(PoolBuffer block)
        {
            var hash = Crc32Algorithm.Compute(block.RawBuffer, 0, block.Length);
            
            if (Table[Counter].Hash != 0)
                Table[Counter].Buffer.Free();

            //Console.WriteLine($"Adding hash {hash:x} {Counter}");
            Table[Counter].Hash = hash;
            Table[Counter].Buffer = block;
            block.Reference();

            Counter++;
            Counter %= HashTableSize;
        }

        public void Flush()
        {
            for (int i = 0; i < HashTableSize; i++)
            {
                if (Table[i].Hash == 0)
                    continue;

                Table[i].Buffer.Free();
                Table[1].Hash = 0;
            }

            Counter = 0;
        }

        public bool LookupHash(uint Hash, out PoolBuffer buffer)
        {
            for (int i = 0; i < HashTableSize; i++) 
            {
                if (Table[i].Hash == Hash)
                {
                    buffer = Table[i].Buffer;
                    buffer.Reference();
                    return true;
                }
            }

            // Should never happen
            buffer = default;
            return false;
        }

        public void Dispose()
        {
            Flush();
        }
    }
}
