using System;
using System.Linq;

namespace Stx.Utilities
{
    public class UniqueCodeGenerator
    {
        private ulong totalIndex = 0;
        private int[,] replacementTable = null;
        private int[] switchBoard = null;

        private int codeLength;
        private string validChars;
        private Random random = new Random();

        /// <summary>
        /// The total number of unique possible combinations this generator will and can generate.
        /// </summary>
        public ulong Possibilities { get; }
        /// <summary>
        /// The index of this generator, each generated code corresponds to an index. See <see cref="GetCodeFor(ulong)"/>.
        /// </summary>
        public ulong Position
        {
            get
            {
                return totalIndex;
            }
        }


        public const string DEFAULT_VALID_CHARACTERS = "abcdefghijklmnopqrstuvwxyz";

        public UniqueCodeGenerator(int codeLength = 4, string validCharacters = DEFAULT_VALID_CHARACTERS, bool useReplacementTable = true, bool useSwitchBoard = true, int? seed = null)
        {
            validChars = validCharacters;
            this.codeLength = codeLength;

            if (seed != null)
                random = new Random(seed.Value);

            if (useReplacementTable)
            {
                replacementTable = new int[codeLength, validCharacters.Length];
                FillReplacementTable();
            }

            if (useSwitchBoard)
            {
                switchBoard = Enumerable.Range(0, codeLength).ToArray();
                switchBoard.Shuffle();
            }

            Possibilities = GetPossibilities();
        }

        public ulong GetPossibilities()
        {
            ulong pow = 1;

            for (int i = 0; i < codeLength; i++)
                pow = pow * (uint)validChars.Length;

            return pow;
        }

        private void FillReplacementTable()
        {
            for(int i = 0; i < replacementTable.GetLength(0); i++)
            {
                var v = Enumerable.Range(0, replacementTable.GetLength(1)).OrderBy(_ => random.Next()).ToArray();

                for (uint j = 0; j < replacementTable.GetLength(1); j++)
                    replacementTable[i, j] = v[j];
            }
        }

        /// <summary>
        /// Returns the next unique code.
        /// </summary>
        /// <param name="increment">The number of codes the generator should skip after generating the returned one.</param>
        /// <returns>An unique code.</returns>
        public string GetNextCode(uint increment = 1)
        {
            return GetCodeFor(totalIndex += increment);
        }

        /// <summary>
        /// Returns a specific code for a specific index. Each code corresponds to an index.
        /// </summary>
        /// <param name="i">The index to generate a code from.</param>
        /// <returns>An unique code that corresponds with the given index.</returns>
        public string GetCodeFor(ulong i)
        {
            int[] index = new int[codeLength];

            for (int k = 0; k < codeLength; k++)
            {
                ulong x = i;
                int l = k;

                for (int j = 0; j < l; j++)
                    x /= (uint)validChars.Length;

                if (switchBoard != null)
                    l = switchBoard[l];

                if (replacementTable == null)
                {
                    index[l] = (int)x % validChars.Length;
                }
                else
                {
                    index[l] = replacementTable[l, (int)x % validChars.Length];
                }
            }

            return string.Join("", index.Select((t) => validChars[(int)t]));
        }
    }
}
