using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stx.Utilities
{
    public static class StringUtil
    {
        public static bool DoMatchOrNull(this string toBeMatched, string matches, bool exact = false)
        {
            if (string.IsNullOrWhiteSpace(matches))
                return true;

            if (exact)
                return string.Compare(toBeMatched, matches, StringComparison.OrdinalIgnoreCase) == 0;
            else
                return toBeMatched.Contains(matches);
        }

        public static bool EqualsIgnoreCase(this string str1, string str2)
        {
            return str1.ToUpper().Trim() == str2.ToUpper().Trim();
        }

        public static string ShortenIfNeeded(string original, int maxSize, string shortenPrefix = "", string shortenSuffix = "")
        {
            if (original.Length <= maxSize)
                return original;

            return Shorten(original, maxSize, shortenPrefix, shortenSuffix);
        }

        public static string Shorten(string original, int maxSize, string shortenPrefix = "", string shortenSuffix = "")
        {
            if (original.Length <= maxSize)
                return shortenPrefix + original + shortenPrefix;

            return shortenPrefix + original.Substring(0, maxSize - shortenPrefix.Length - shortenSuffix.Length) + shortenSuffix;
        }

        public static string FormatBytes(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }

        public static long StringToUniqueLong(string str)
        {
            byte[] a = Encoding.ASCII.GetBytes(str);
            long l =0 ;
            for (int i = 0; i < a.Length; i++)
                l -= ((long)Math.Pow(128, i)) + a[i];
                
            return l / str.Length;
        }
    }
}
