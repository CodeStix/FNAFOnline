using System.Collections.Generic;

namespace Stx.Serialization
{
    public class BList<T> : List<T>, IBytifiable, IByteDefined
    {
        public BList()
        { }

        public BList(IEnumerable<T> collection) : base(collection)
        { }

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
