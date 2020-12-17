using Stx.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace Stx.Serialization
{
    public class BDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IBytifiable, IByteDefined
    {
        public void FromBytes(byte[] from)
        {
            Stack<byte[]> b = ByteUtil.ToSegmentStack(from);

            var k = Bytifier.BytesToArray<TKey>(b.Pop());
            var v = Bytifier.BytesToArray<TValue>(b.Pop());

            for (int i = 0; i < k.Length; i++)
                Add(k[i], v[i]);
        }

        public byte[] ToBytes()
        {
            Stack<byte[]> b = new Stack<byte[]>();

            b.Push(Bytifier.ArrayToBytes(Keys.ToArray()));
            b.Push(Bytifier.ArrayToBytes(Values.ToArray()));

            return ByteUtil.FromSegmentStack(b);
        }
    }
}
