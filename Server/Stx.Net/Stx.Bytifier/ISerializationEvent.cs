using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stx.Serialization
{
    public interface ISerializationEvent
    {
        void BeforeSerialize();
        void AfterDeserialize();
    }
}
