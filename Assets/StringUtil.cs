using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public static class StringUtil
{
    public const string ILLEGAL_IO_CHARS = "#/*+?|<>:;[]{}\"";
    public const string ABC = "abcdefghijklmnopqrstuvwxyz";

    public static int FirstIndexOfAny(string str, params string[] s)
    {
        int lowest = int.MaxValue;
        foreach (string ss in s)
        {
            int i = str.IndexOf(ss);
            if (i < 0)
                continue;
            if (i < lowest)
            {
                lowest = i;
            }
        }
        if (lowest == int.MaxValue)
            return -1;
        else
            return lowest;
    }
        
    public static bool ContainsIllegalCharacter(string str, string illegalChars = ILLEGAL_IO_CHARS)
    {
        foreach (char c in illegalChars)
            if (illegalChars.Contains(c))
                return true;
        return false;
    }

    public static string RemoveIllegalCharacters(string str, string illegalChars = ILLEGAL_IO_CHARS)
    {
        foreach (char c in illegalChars)
            str = str.Replace(c.ToString(), "");
        return str;
    }

    public static bool IsSimular(string str1, string str2, int maxDifferences = 10)
    {
        str1 = GetShortest(str1, str2).Trim();
        str2 = GetLongest(str2, str1).Trim();

        int[] str1Chars = new int[ABC.Length];
        for(int i = 0; i < str1.Length; i++)
        {
            int index = ABC.IndexOf(str1[i]);

            if (index >= 0)
                str1Chars[index]++;
        }

        int[] str2Chars = new int[ABC.Length];
        for (int i = 0; i < str2.Length; i++)
        {
            int index = ABC.IndexOf(str2[i]);

            if (index >= 0)
                str2Chars[index]++;
        }

        int differences = 0;

        for(int i = 0; i < ABC.Length; i++)
        {
            differences += Math.Abs(str1Chars[i] - str2Chars[i]);
        }

        return true;
    }

    public static string GetShortest(string str1, string str2)
    {
        if (str2.Length < str1.Length)
            return str2;
        else
            return str1;
    }

    public static string GetLongest(string str1, string str2)
    {
        if (str2.Length > str1.Length)
            return str2;
        else
            return str1;
    }

    public static string Repeat(string str, int times)
    {
        string s = "";
        for (int i = 0; i < times; i++)
            s += str;
        return s;
    }

    public static string RemoveFromBracketsToEnd(string str)
    {
        if (str.Contains("("))
            str = str.Substring(0, str.IndexOf("("));
        if (str.Contains("["))
            str = str.Substring(0, str.IndexOf("["));
        if (str.Contains("{"))
            str = str.Substring(0, str.IndexOf("{"));
        return str;
    }

    public static bool ExtractBoolOrDefault(string str, bool defaultBool = false)
    {
        if (string.IsNullOrEmpty(str))
            return defaultBool;
        str = str.Trim().ToLower().Substring(0,1);
        if (str == "y" || str == "1" || str == "t")
            return true;
        else if (str == "n" || str == "0" || str == "f")
            return false;
        else
            return defaultBool;
    }

    public static bool IsUrl(string str)
    {
        str = str.Trim().ToLower();
        return str.StartsWith("http://") || str.StartsWith("https://") || str.StartsWith("www.");
    }

    public static bool ContainsAny(string str, params string[] contains)
    {
        foreach (string c in contains)
            if (str.Contains(c))
                return true;
        return false;
    }

    public static string SurroundWithQuotes(string str)
    {
        if (!str.StartsWith("\""))
            str = "\"" + str;
        if (!str.EndsWith("\""))
            str += "\"";
        return str;
    }


}

