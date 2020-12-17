using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stx.Serialization
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class DoNotSerializeAttribute : Attribute
    { }
}
