using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stx.Serialization
{
    /// <summary>
    /// Attach this interface to every object you want to serialize using <see cref="Stx.Serialization.Bytifier"/>.
    /// <para>You don't have to implement anything, just make sure there is a parameterless constructor.</para>
    /// <para>Make sure you include it via <see cref="Bytifier.Include{T}"/>.</para>
    /// </summary>
    public interface IByteDefined
    { }

    /// <summary>
    /// Attach this interface to every object you want to serialize using <see cref="Stx.Serialization.Bytifier"/>.
    /// <para>You don't have to implement anything, just make sure there is a parameterless constructor.</para>
    /// <para>Make sure you include it via <see cref="Bytifier.Include{T}"/>.</para>
    /// </summary>
    public interface IByteDefined<T> : IByteDefined where T : new()
    { }

    /// <summary>
    /// Attach this interface to a object you want to manually convert to bytes, this enhances speed and size.
    /// <para>Make sure you include it via <see cref="Bytifier.Include{T}"/> and that a parameterless constructor exists!</para>
    /// You should take a look at <see cref="Stx.Utilities.ByteUtil"/>. 
    /// </summary>
    public interface IBytifiable : IByteDefined
    {
        void FromBytes(byte[] from);
        byte[] ToBytes();
    }

    /// <summary>
    /// Attach this interface to a object you want to manually convert to bytes, this enhances speed and size.
    /// <para>Make sure you include it via <see cref="Bytifier.Include{T}"/> and that a parameterless constructor exists!</para>
    /// You should take a look at <see cref="Stx.Utilities.ByteUtil"/>. 
    /// </summary>
    public interface IBytifiable<T> : IBytifiable where T : new()
    { }
}
