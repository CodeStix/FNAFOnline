using System;
using System.Collections.Generic;

namespace Stx.Utilities
{
    public static class ByteUtil
    {
        // Format of SegmentStack:
        // [0:segmentCount] [2->(segmentCount*4): segmentSize] ... [dataSegment1] [dataSegment2] ... [dataSegmentN]

        /// <summary>
        /// Creates a stack of one-dimensional byte arrays. Opposite of <see cref="FromSegmentStack(Stack{byte[]})"/>.
        /// </summary>
        /// <param name="buffer">The compatible byte array (created with <see cref="FromSegmentStack(Stack{byte[]})"/>) to convert back to a byte array stack.</param>
        /// <returns>The stack you just converted.</returns>
        public static Stack<byte[]> ToSegmentStack(byte[] buffer, int index, int count)
        {
            if (count == 0)
                return new Stack<byte[]>();

            byte[] b = new byte[count];
            Array.Copy(buffer, index, b, 0, count);
            return ToSegmentStack(b);
        }

        /// <summary>
        /// Creates a stack of one-dimensional byte arrays. Opposite of <see cref="FromSegmentStack(Stack{byte[]})"/>.
        /// </summary>
        /// <param name="buffer">The compatible byte array (created with <see cref="FromSegmentStack(Stack{byte[]})"/>) to convert back to a byte array stack.</param>
        /// <returns>The stack you just converted.</returns>
        public static Stack<byte[]> ToSegmentStack(byte[] buffer)
        {
            ushort segmentCount = BitConverter.ToUInt16(buffer,0);
            int[] segmentSizes = new int[segmentCount];
            for (int i = 0; i < segmentCount; i++)
                segmentSizes[i] = BitConverter.ToInt32(buffer, i * 4 + 2);

            Stack<byte[]> stack = new Stack<byte[]>();

            int reader = segmentCount * 4 + 2;
            byte[] currentBuffer;
            for (int i = 0; i < segmentCount; i++)
            {
                currentBuffer = new byte[segmentSizes[i]];

                Array.Copy(buffer, reader, currentBuffer, 0, currentBuffer.Length);
                reader += currentBuffer.Length;

                stack.Push(currentBuffer);
            }

            return stack;
        }

        /// <summary>
        /// Create a one-dimensional byte array from a <see cref="Stack{T}"/> of bytes. Opposite is <see cref="ToSegmentStack(byte[])"/>.
        /// <para>The size of byte arrays inside the stack may vary. The stack doesn't have a fixed size.</para>
        /// <para>This is handy if you want to store multiple sets of data in the same byte array without complicated calculations.</para>
        /// </summary>
        /// <param name="stack">The stack of byte arrays to convert.</param>
        /// <returns>A one-dimensional byte array containing all the stacks data. Convert it back with <see cref="ToSegmentStack(byte[])"/>.</returns>
        public static byte[] FromSegmentStack(Stack<byte[]> stack)
        {
            long totalSize = 0;
            foreach (byte[] v in stack)
                totalSize += v.Length;

            ushort segmentCount = (ushort)stack.Count;
            byte[] tbuffer = new byte[totalSize + 4 * segmentCount + 2];
            Array.Copy(BitConverter.GetBytes(segmentCount), 0, tbuffer, 0, 2);

            int k = 2;
            int reader = segmentCount * 4 + 2;
            while (stack.Count > 0)
            {
                byte[] p = stack.Pop();

                Array.Copy(BitConverter.GetBytes(p.Length), 0, tbuffer, k, 4);
                Array.Copy(p, 0, tbuffer, reader, p.Length);

                reader += p.Length;
                k += 4;
            }

            return tbuffer;
        }

        /// <summary>
        /// Used to prevent combined buffers in a byte stream (for example <see cref="System.Net.Sockets.Socket"/>).
        /// </summary>
        /// <param name="buffer">The whole buffer to unwrap if needed.</param>
        /// <returns>One or multiple buffers to interpret.</returns>
        public static IEnumerable<byte[]> UnwrapSegmentedBytes(byte[] buffer)
        {
            int cl = BitConverter.ToInt32(buffer, 0);
            int ci = 0;
            byte[] c = new byte[cl];

            for (int i = 4; i < buffer.Length; i++)
            {
                c[ci++] = buffer[i];

                if (ci >= cl)
                {
                    yield return c;

                    if (buffer.Length - i >= 4)
                    {
                        cl = BitConverter.ToInt32(buffer, i + 1);
                        if (cl <= 0 || cl > buffer.Length - i)
                            yield break;
                        i += 4;
                        ci = 0;
                        c = new byte[cl];
                    }
                    else
                    {
                        yield break;
                    }
                }
            }
        }

        /// <summary>
        /// Wrap a buffer so it is compatible with <see cref="UnwrapSegmentedBytes(byte[])"/>.
        /// </summary>
        /// <param name="buffer">The buffer to wrap.</param>
        /// <returns>The modified buffer.</returns>
        public static byte[] WrapSegmentedBytes(byte[] buffer)
        {
            byte[] b = new byte[buffer.Length + 4];
            Array.Copy(buffer, 0, b, 4, buffer.Length);
            Array.Copy(BitConverter.GetBytes(buffer.Length), 0, b, 0, 4);

            return b;
        }

        /*public static void VisualizeBytesToConsole(byte[] b)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            if (b.Length < 6)
            {
                Console.WriteLine("Length < 6");
                Console.ResetColor();
                return;
            }
            int l = BitConverter.ToInt32(b, 0);
            Console.Write(BytesToString(b, 0, 4)  + "\t:\t" + l + Environment.NewLine);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(BytesToString(b, 4, 1) + "\t:\t" + b[4] + Environment.NewLine);
            Console.ResetColor();
        }*/

        public static string BytesToString(int newLineBytes = 0, params byte[] b)
        {
            string str = "";

            for (int i = 0; i < b.Length; i++)
            {
                str += Convert.ToString(b[i], 2).PadLeft(8, '0') + " ";

                if ((newLineBytes == 1 || i % newLineBytes == 0) && newLineBytes != 0 && i != 0)
                    str += Environment.NewLine;
            }

            return str;
        }

        public static string BytesToString(params byte[] b)
        {
            string str = "";

            for (int i = 0; i < b.Length; i++)
                str += Convert.ToString(b[i], 2).PadLeft(8, '0') + " ";

            return str;
        }

        public static string BytesToString(byte[] b, int start, int count)
        {
            string str = "";

            for (int i = start; i < start + count && i < b.Length; i++)
                str += Convert.ToString(b[i], 2).PadLeft(8, '0') + " ";

            return str;
        }


        public static byte[] Xor(byte[] b, byte with)
        {
            for (int i = 0; i < b.Length; i++)
                b[i] ^= with;

            return b;
        }
    }
}
