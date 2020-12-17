using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stx.Utilities
{
    public static class StringChecker
    {
        public const string LowerAlpha = "abcdefghijklmnopqrstuvwxyz";
        public const string UpperAlpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const string Numbers = "0123456789";

        public const string ValidNameChars = LowerAlpha + UpperAlpha + "_-'éèëäáà" + Numbers;
        public const string ValidFullNameChars = ValidNameChars + " .";
        public const string ValidPasswordChars = "_-+*/!@ " + LowerAlpha + UpperAlpha + Numbers;
        public const string ValidUUIDChars = "-_{}" + LowerAlpha + UpperAlpha + Numbers;
        public const string ValidVersionChars = "^vV.*" + Numbers;

        public static bool IsLength(string str, int min, int max)
        {
            return str.Length >= min || str.Length <= max;
        }

        public static bool Matches(string str, string charCollection)
        {
            foreach (char c in str)
                if (!charCollection.Contains(c))
                    return false;
            return true;
        }

        public static bool IsValidApp(string appName, string appVersion)
        {
            return IsValidAppName(appName) && IsValidVersion(appVersion);
        }

        public static bool IsValidAppName(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            return IsLength(str, 3, 30) && Matches(str, ValidFullNameChars);
        }

        public static bool IsValidVersion(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            return IsLength(str, 3, 9) && Matches(str, ValidVersionChars);
        }

        public static bool IsValidName(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            return IsLength(str, 3, 13) && Matches(str, ValidNameChars);
        }

        public static bool IsValidFullName(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            return IsLength(str, 3, 20) && Matches(str, ValidFullNameChars);
        }

        public static bool IsValidID(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            return IsLength(str, 16, 64) && Matches(str, ValidUUIDChars);
        }

        public static bool IsValidPassword(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            return IsLength(str, 4, 64) && Matches(str, ValidPasswordChars);
        }

        public static bool IsValidShortPassword(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            return IsLength(str, 3, 24) && Matches(str, ValidPasswordChars);
        }
    }
}
