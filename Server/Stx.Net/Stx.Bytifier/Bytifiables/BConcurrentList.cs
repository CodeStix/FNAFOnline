using System.Collections.Generic;
using Stx.Collections.Concurrent;

namespace Stx.Serialization
{
    public class BConcurrentList<T> : ConcurrentList<T>, IBytifiable, IByteDefined
    {
        public void FromBytes(byte[] from)
        {
            AddRange(Bytifier.BytesToArray<T>(from));
        }

        public byte[] ToBytes()
        {
            return Bytifier.ArrayToBytes(ToArray());
        }
    }
}
