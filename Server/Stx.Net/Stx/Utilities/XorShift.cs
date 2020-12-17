using System;
using System.Text;

namespace Stx.Utilities
{
    /// <summary>
    /// XorShift random number generators are a class of pseudo-random number generators that were discovered by George Marsaglia.
    /// </summary>
    public static class XorShift
    {
        public static long State { get; set; } = DateTime.Now.Ticks;

        public static void Initiate(long state)
        {
            State = state;
            WowStates = new int[5] { (int)state + 3, (int)state - 87, (int)state - 3, (int)state + 2, (int)state * 8 };
            PlusStates = new long[2] { state - 102, state / 2 };
        }
        
        public static byte NextByte(byte shift = 3)
        {
            return (byte)(NextInt() >> shift);
        }

        public static short NextShort()
        {
            return (short)NextInt();
        }

        public static int NextInt(int includedMin, int excludedMax)
            => includedMin + (Math.Abs(NextInt()) % (excludedMax - includedMin));

        public static int NextInt()
        {
            int x = (int)State;
            x ^= x << 13;
            x ^= x >> 7;
            x ^= x << 17;
            State = x;
            return x;
        }

        public static long NextLong()
        {
            long x = State;
            x ^= x << 13;
            x ^= x >> 7;
            x ^= x << 17;
            State = x;
            return x;
        }

        public static int[] WowStates { get; set; } = new int[5] { 9, 87, 3, 2, 8 };

        public static int NextIntWow()
        {
            int s, t = WowStates[3];
            t ^= t >> 2;
            t ^= t << 1;
            WowStates[3] = WowStates[2]; WowStates[2] = WowStates[1]; WowStates[1] = s = WowStates[0];
            t ^= s;
            t ^= s << 4;
            WowStates[0] = t;
            return t + (WowStates[4] += 362437);
        }

        public static long NextLongStar()
        {
            long x = State;
            x ^= x >> 12;
            x ^= x << 25;
            x ^= x >> 27;
            State = x;
            return x * 0x2545F4914F6CDD1D;
        }

        public static long[] PlusStates { get; set; } = new long[2] { 5, 6 };

        public static long NextLongPlus()
        {
            long x = PlusStates[0];
            long y = PlusStates[1];
            PlusStates[0] = y;
            x ^= x << 23;
            PlusStates[1] = x ^ y ^ (x >> 17) ^ (y >> 26);
            return PlusStates[1] + y;
        }
    }
}
