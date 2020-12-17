using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Stx.Utilities.ErrorHandling;

namespace Stx.Utilities
{
    public static class ExtensionMethods
    {
        private static Random rng = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static void Modify(this Hashtable dir, string key, object value)
        {
            if (dir.ContainsKey(key))
                dir[key] = value;
            else
                dir.Add(key, value);
        }

        public static object Get(this Hashtable dir, string key, object defaultValue = null)
        {
            if (dir.ContainsKey(key))
                return dir[key];
            else
                return defaultValue;
        }

        /*public static bool ContainsType<T>(this Dictionary<string, object> dir, string key)
        {
            foreach (KeyValuePair<string, object> e in dir)
                if (e.Key == key && e.Value.GetType() == typeof(T))
                    return true;

            return false;
        }*/

        public static T Get<T>(this Hashtable dir, string key, T defaultValue = default(T))
        {
            if (dir.ContainsKey(key))
            {
                var v = dir[key];

                if (v == null)
                    return defaultValue;

                if (v is T)
                    return (T)dir[key];

                return defaultValue;
            }
            else
            {
                return defaultValue;
            }
        }

        /*public static object Get(this Hashtable dir, string key, object defaultValue = null)
        {
            if (dir.ContainsKey(key))
                return dir[key];
            else
                return defaultValue;
        }*/

        public static bool ContainsType(this Hashtable dir, string key, Type valueType)
        {
            if (string.IsNullOrEmpty(key))
                return false;

            if (dir.ContainsKey(key))
            {
                if (dir[key] == null || valueType == null)
                    return dir[key] == null && valueType == null;

                if (valueType.IsEnum && (dir[key].GetType() == Enum.GetUnderlyingType(valueType) || dir[key].GetType() == valueType))
                    return true;

                return valueType.IsAssignableFrom(dir[key].GetType());
            }
            else
            {
                return false;
            }
        }

        public static bool Requires<T>(this Hashtable dir, string key)
        {
            return ContainsType(dir, key, typeof(T));
        }

        public static bool Requires<T0, T1>(this Hashtable dir, string key0, string key1)
        {
            if (!Requires<T0>(dir, key0) || !Requires<T1>(dir, key1))
                return false;

            return true;
        }

        public static bool Requires<T0, T1, T2>(this Hashtable dir, string key0, string key1, string key2)
        {
            if (!Requires<T0>(dir, key0) || !Requires<T1>(dir, key1) || !Requires<T2>(dir, key2))
                return false;

            return true;
        }

        public static bool Requires<T0, T1, T2, T3>(this Hashtable dir, string key0, string key1, string key2, string key3)
        {
            if (!Requires<T0>(dir, key0) || !Requires<T1>(dir, key1) || !Requires<T2>(dir, key2) || !Requires<T3>(dir, key3))
                return false;

            return true;
        }

        public static bool Requires<T0, T1, T2, T3, T4>(this Hashtable dir, string key0, string key1, string key2, string key3, string key4)
        {
            if (!Requires<T0>(dir, key0) || !Requires<T1>(dir, key1) || !Requires<T2>(dir, key2) || !Requires<T3>(dir, key3) || !Requires<T4>(dir, key4))
                return false;

            return true;
        }

        public static bool Requires<T0, T1, T2, T3, T4, T5>(this Hashtable dir, string key0, string key1, string key2, string key3, string key4, string key5)
        {
            if (!Requires<T0>(dir, key0) || !Requires<T1>(dir, key1) || !Requires<T2>(dir, key2) || !Requires<T3>(dir, key3) || !Requires<T4>(dir, key4) || !Requires<T5>(dir, key5))
                return false;

            return true;
        }

        public static bool Requires<T0, T1, T2, T3, T4, T5, T6>(this Hashtable dir, string key0, string key1, string key2, string key3, string key4, string key5, string key6)
        {
            if (!Requires<T0>(dir, key0) || !Requires<T1>(dir, key1) || !Requires<T2>(dir, key2) || !Requires<T3>(dir, key3) || !Requires<T4>(dir, key4) || !Requires<T5>(dir, key5) || !Requires<T6>(dir, key6))
                return false;

            return true;
        }

        public static bool Requires<T0, T1, T2, T3, T4, T5, T6, T7>(this Hashtable dir, string key0, string key1, string key2, string key3, string key4, string key5, string key6, string key7)
        {
            if (!Requires<T0>(dir, key0) || !Requires<T1>(dir, key1) || !Requires<T2>(dir, key2) || !Requires<T3>(dir, key3) || !Requires<T4>(dir, key4) || !Requires<T5>(dir, key5) || !Requires<T6>(dir, key6) || !Requires<T7>(dir, key7))
                return false;

            return true;
        }

        public static bool Requires(this Hashtable dir, string[] keys, Type[] valueTypes)
        {
            for (int i = 0; i < keys.Length; i++)
                if (!ContainsType(dir, keys[i], valueTypes[i]))
                    return false;

            return true;
        }
    }
}
