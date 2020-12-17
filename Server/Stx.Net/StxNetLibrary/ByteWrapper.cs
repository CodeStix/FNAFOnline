using Stx.Utilities;
using System;

namespace Stx.Net
{
    /// <summary>
    /// A byte wrapper of format [0-1:Crc16] [2-3:Type] [4..:Data]
    /// </summary>
    public class ByteWrapper
    {
        public byte[] RawBuffer { get; set; }
        public byte[] DataBuffer { get; set; }
        public ushort FirstCrc16 { get; }
        public ushort LastCrc16 { get; }
        public ushort Type { get; }
        public BytesContentType? ContentType { get; } = null;
        public bool Integrity { get; }

        private static Crc16 crcCalculator = new Crc16();

        public ByteWrapper(byte[] wrappedDataBytes)
        {
            RawBuffer = wrappedDataBytes;

            if (wrappedDataBytes.Length >= 4)
            {
                FirstCrc16 = BitConverter.ToUInt16(wrappedDataBytes, 0);
                Type = BitConverter.ToUInt16(wrappedDataBytes, 2);
                ContentType = (BytesContentType)Type;
                DataBuffer = new byte[wrappedDataBytes.Length - 4];
                Array.Copy(wrappedDataBytes, 4, DataBuffer, 0, DataBuffer.Length);

                wrappedDataBytes[0] = 0x0;
                wrappedDataBytes[1] = 0x0;
                LastCrc16 = crcCalculator.ComputeChecksum(wrappedDataBytes);
                Integrity = LastCrc16 == FirstCrc16;
            }
            else
            {
                Type = 0;
                ContentType = BytesContentType.Nothing;
                DataBuffer = new byte[0];

                Integrity = false;
            }
         
        }

        public static byte[] Wrap(byte[] dataBytes, BytesContentType contentType)
            => Wrap(dataBytes, (ushort)contentType);

        public static byte[] Wrap(byte[] dataBytes, ushort type)
        {
            byte[] buffer = new byte[dataBytes.Length + 4];
            Array.Copy(BitConverter.GetBytes(type), 0, buffer, 2, 2);
            Array.Copy(dataBytes, 0, buffer, 4, dataBytes.Length);
            Array.Copy(crcCalculator.ComputeChecksumBytes(buffer), 0, buffer, 0, 2);
            return buffer;
        }

        public static ByteWrapper UnWrap(byte[] wrappedDataBytes)
        {
            return new ByteWrapper(wrappedDataBytes);
        }
    }
}
