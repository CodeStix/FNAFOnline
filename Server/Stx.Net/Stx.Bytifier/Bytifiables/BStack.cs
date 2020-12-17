using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stx.Serialization
{
    public class BStack<T> : Stack<T>, IBytifiable, IByteDefined
    {
        public void FromBytes(byte[] from)
        {
            T[] v = Bytifier.BytesToArray<T>(from);

            for (int i = v.Length - 1; i >= 0; i--)
                Push(v[i]);
        }

        public byte[] ToBytes()
        {
            return Bytifier.ArrayToBytes(ToArray());
        }
    }
}
