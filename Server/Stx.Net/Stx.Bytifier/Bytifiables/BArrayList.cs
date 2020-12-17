using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stx.Serialization.Bytifiables
{
    public class BArrayList : ArrayList, IBytifiable, IByteDefined
    {
        public void FromBytes(byte[] from)
        {
            AddRange(Bytifier.BytesToArray<object>(from));
        }

        public byte[] ToBytes()
        {
            return Bytifier.ArrayToBytes(ToArray());
        }
    }
}
