using Stx.Utilities;
using System.Collections;
using System.Collections.Generic;

namespace Stx.Serialization
{
    public class BHashtable : Hashtable, IBytifiable
    {
        public BHashtable() : base()
        { }

        public BHashtable(Hashtable from) : base(from)
        { }

        public BHashtable(byte[] from)
        {
            FromBytes(from);
        }

        public void FromBytes(byte[] from)
        {
            Stack<byte[]> b = ByteUtil.ToSegmentStack(from);

            var k = Bytifier.BytesToArray<object>(b.Pop());
            var v = Bytifier.BytesToArray<object>(b.Pop());

            for (int i = 0; i < k.Length; i++)
                Add(k[i], v[i]);
        }

        public byte[] ToBytes()
        {
            Stack<byte[]> b = new Stack<byte[]>();

            object[] keys = new object[base.Keys.Count];
            object[] values = new object[base.Values.Count];
            base.Keys.CopyTo(keys, 0);
            base.Values.CopyTo(values, 0);
            b.Push(Bytifier.ArrayToBytes(keys, true));
            b.Push(Bytifier.ArrayToBytes(values, true));

            return ByteUtil.FromSegmentStack(b);
        }
    }

}
