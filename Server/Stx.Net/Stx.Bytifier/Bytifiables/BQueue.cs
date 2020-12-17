using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stx.Serialization
{
    public class BQueue<T> : Queue<T>, IBytifiable, IByteDefined
    {
        public void FromBytes(byte[] from)
        {
            T[] v = Bytifier.BytesToArray<T>(from);

            for (int i = 0; i < v.Length; i++)
                Enqueue(v[i]);
        }

        public byte[] ToBytes()
        {
            return Bytifier.ArrayToBytes(ToArray());
        }
    }
}
