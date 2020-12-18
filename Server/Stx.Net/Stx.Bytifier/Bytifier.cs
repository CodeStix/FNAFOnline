using Stx.Logging;
using Stx.Utilities;
using Stx.Utilities.ErrorHandling;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Stx.Serialization
{
    /// <summary>
    /// Object serializing system used to send objects over network. Does not support version control.
    /// </summary>
    public static class Bytifier
    {
        public static ConcurrentDictionary<byte, Type> CustomTypes { get; set; } = new ConcurrentDictionary<byte, Type>();
        public static Encoding StringEncoding { get; set; } = Encoding.ASCII;
        public static byte StringXorKey { get; set; } = 120;
        public static bool SerializeStaticMembers { get; set; } = false;
        public static bool SerializePrivateMembers { get; set; } = true;
        public static bool SerializeInheritedMembers { get; set; } = true;

        public static ILogger Logger { get; set; } = new VoidLogger();

        private static byte registerIndex = 255;
        private static readonly byte[] nullBytes = new byte[] { 0x0, 0xff, 0x0, 0xff };

        private const byte InternalTypeBorder = 20;

        private static BindingFlags GetBindingFlags()
        {
            return BindingFlags.Public 
                | BindingFlags.Instance 
                | (SerializeStaticMembers ? BindingFlags.Static : BindingFlags.Default)
                | (SerializeInheritedMembers ? BindingFlags.Default : BindingFlags.DeclaredOnly)
                | (SerializePrivateMembers ? BindingFlags.NonPublic : BindingFlags.Default);
        }

        /// <summary>
        /// Includes a type with code <paramref name="typeCode"/>, make sure they are the same when serializing and/or deserializing.
        /// </summary>
        /// <param name="type">The type to include.</param>
        /// <param name="typeCode">Type code to bind to that type. Must be unique for each included type and must be between 21 and 255.</param>
        public static void IncludeManually(Type type, byte typeCode)
        {
            if (!typeof(IByteDefined).IsAssignableFrom(type) && !type.IsEnum)
            {
                Logger.Log($"Please mark type { type } with the { nameof(IByteDefined) } interface.", LoggedImportance.CriticalError);
                return;
            }
            if (typeCode <= InternalTypeBorder)
            {
                Logger.Log($"Please pick a higher type-code than { typeCode }, minimun is { InternalTypeBorder + 1 }.", LoggedImportance.CriticalError);
                return;
            }

            if (!CustomTypes.ContainsKey(typeCode))
                CustomTypes.TryAdd(typeCode, type);
            else
                CustomTypes[typeCode] = type;
        }

        public static void IncludeManually<T>(byte typeCode)
        {
            IncludeManually(typeof(T), typeCode);
        }


        public static byte Include(Type type)
        {
            if (registerIndex <= InternalTypeBorder)
            {
                Logger.Log("The include border has been reached, please remove some unnecessary includes.", LoggedImportance.CriticalError);
                return 0;
            }

            IncludeManually(type, registerIndex);
            return registerIndex--;
        }

        /// <summary>
        /// Includes a type in the serialization process. 
        /// <para>!! Make sure you use the same include order in the deserialization process.</para>
        /// </summary>
        /// <typeparam name="T">The type to include in the serialization process.</typeparam>
        /// <returns>The type code of the included type, automatically determined.</returns>
        public static byte Include<T>() where T : IByteDefined
        {
            return Include(typeof(T));
        }

        public static byte IncludeEnum(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                Logger.Log($"Type { enumType } is not an enum.", LoggedImportance.CriticalError);
                return 0;
            }

            IncludeManually(enumType, registerIndex);
            return registerIndex--;
        }

        public static byte IncludeEnum<T>()
        {
            return IncludeEnum(typeof(T));
        }

        public static byte[] Bytes(object b)
        {
            if (b is ISerializationEvent)
                ((ISerializationEvent)b).BeforeSerialize();

            if (b is IBytifiable)
                return ((IBytifiable)b).ToBytes();

            Type t = b.GetType();

            if (t.IsEnum)
                return BitConverter.GetBytes((Int32)b);

            Stack<byte[]> buffers = new Stack<byte[]>();
            long total = 0;

            PropertyInfo[] properties = t.GetProperties(GetBindingFlags());
            foreach (PropertyInfo property in properties)
            {
                if (Attribute.IsDefined(property, typeof(DoNotSerializeAttribute)))
                    continue;

                // The value of the current reading property.
                object v = property.GetValue(b);

                byte vtc = 0;
                byte[] buffer = SingleObject(v, out vtc);

                if (buffer == null)
                {
                    Logger.Log("Could not serialize: a non-included type was found, please include.", LoggedImportance.CriticalError);
                    return null;
                }

                byte[] tbuffer = new byte[buffer.Length + 5];

                Array.Copy(BitConverter.GetBytes(buffer.Length), 0, tbuffer, 1, 4);
                Array.Copy(buffer, 0, tbuffer, 5, buffer.Length);
                tbuffer[0] = vtc;

                buffers.Push(tbuffer);
                total += tbuffer.Length;
            }

            buffers.Push(new byte[] { TypeToCode(t) });
            total++;

            return CombineStack(buffers, total);
        }

        public static byte[] CombineStack(Stack<byte[]> buffers, long totalBytes)
        {
            byte[] total = new byte[totalBytes];
            int pos = 0;
            while (buffers.Count > 0)
            {
                byte[] bt = buffers.Pop();
                Array.Copy(bt, 0, total, pos, bt.Length);
                pos += bt.Length;
            }

            return total;
        }

        public static object AnyObject(byte[] b)
        {
            // The objects type-code.
            byte tc = b[0];

            if (tc == 0x0 || !CustomTypes.ContainsKey(tc))
            {
                Logger.Log("The serialized input did not provide a type code, please include on the serializing side.", LoggedImportance.CriticalError);
                return null;
            }

            Type t = CustomTypes[tc];

            MethodInfo method = typeof(Bytifier).GetMethod(nameof(Object));
            MethodInfo generic = method.MakeGenericMethod(t);
            return generic.Invoke(null, new object[] { b });
        }

        public static T Object<T>(byte[] b) where T : new()
            => ObjectSize<T>(b, 0, b.Length);

        public static T ObjectSize<T>(byte[] b, int start) where T : new()
            => ObjectSize<T>(b, start, b.Length);

        public static T ObjectSize<T>(byte[] b, int start, int count) where T : new()
        {
            T a = new T();

            if (a is IBytifiable)
            {
                ((IBytifiable)a).FromBytes(b);
                return a;
            }
             
            Stack<byte[]> buffers = new Stack<byte[]>();
            Type t = CodeToType(b[start]);

            if (a.GetType() != t && t != null)
                a = (T)FastActivator.CreateInstance(t);
            if (t == null)
                t = a.GetType();

            // The current items type-code.
            byte ctc = 0;
            // To check if the reader should now read a new piece of object data.
            bool doNew = true;
            // The length of the current object.
            int len = 0;
            // The current reading position of the current object.
            int c = 0;
            // The current reading buffer.
            byte[] buffer = new byte[1];
            // To check if the reader has to read bytes into the current buffer.
            bool doRead = false;
            
            PropertyInfo[] properties = t.GetProperties(GetBindingFlags());
            // Index of the current item property.
            int prop = properties.Length - 1;

            for (int i = start + 1; i < count; i++)
            {
                byte red = b[i];

                if (doRead)
                {
                    if (buffer.Length > 0)
                        buffer[c] = red;

                    c++;

                    if (c >= len)
                    {
                        object o = SingleBuffer(buffer, ctc);

                        //Skipping properties with the DoNotSerialize attribute
                        while (prop >= 0 && Attribute.IsDefined(properties[prop], typeof(DoNotSerializeAttribute)))
                            prop--;

                        if (prop < 0)
                        {
                            Logger.Log($"The end object did not provide the next property: setting for type { t }; type { typeof(T) } is provided.", LoggedImportance.CriticalWarning);

                            break;
                        }

                        try
                        {
                            properties[prop].SetValue(a, o);
                        }
                        catch(Exception e)
                        {
                            Logger.LogException(e, "Could not set property");
                        }

                        //Console.WriteLine($"{ properties[prop].Name } = ({ o?.GetType() }){ o?.ToString() }");

                        prop--;
                        doRead = false;
                        doNew = true;
                        continue;
                    }
                }

                if (doNew)
                {
                    doNew = false;
                    doRead = true;
                    ctc = red;
                    len = BitConverter.ToInt32(b, i + 1);
                    buffer = new byte[len];
                    c = 0;
                    i += 4;

                    //Console.Write($"New reader with length { len } for object type { ctc }");
                }

            }

            if (a is ISerializationEvent)
                ((ISerializationEvent)a).AfterDeserialize();

            return a;
        }

        public static object SingleBuffer(byte[] buffer, byte typeCode)
        {
            switch (typeCode)
            {
                case 1:
                    return null;

                case 2:
                    return buffer[0];
                case 3:
                    return BitConverter.ToChar(buffer, 0);
                case 4:
                    return BitConverter.ToBoolean(buffer, 0);
                case 5:
                    return BitConverter.ToInt16(buffer, 0);
                case 6:
                    return BitConverter.ToInt32(buffer, 0);
                case 7:
                    return BitConverter.ToInt64(buffer, 0);
                case 8:
                    return BytesToString(buffer);
                case 9:
                    return BitConverter.ToDouble(buffer, 0);
                case 10:
                    return BitConverter.ToSingle(buffer, 0);
                case 11:
                    return BitConverter.ToUInt16(buffer, 0);
                case 12:
                    return BitConverter.ToUInt32(buffer, 0);
                case 13:
                    return BitConverter.ToUInt64(buffer, 0);
                /*case 14:
                    return BitConverter.ToInt32(buffer, 0);*/
                case 15:
                    return AnyBytesToArray(buffer);
                case 16:
                    return DateTime.FromBinary(BitConverter.ToInt64(buffer, 0));
                case 17:
                    return TimeSpan.FromTicks(BitConverter.ToInt64(buffer, 0));

                default:
                    if (CustomTypes.ContainsKey(typeCode))
                    {
                        Type custom = CustomTypes[typeCode];
                        object result = null;

                        if (custom.IsEnum)
                        {
                            result = Enum.ToObject(custom, BitConverter.ToInt32(buffer, 0));
                        }
                        else
                        {
                            object o = FastActivator.CreateInstance(custom);

                            if (o is IBytifiable)
                            {
                                IBytifiable ib = (IBytifiable)o;
                                ib.FromBytes(buffer);
                                result = ib;
                            }
                            else
                            {
                                result = AnyObject(buffer);
                            }
                        }

                        return result;
                    }
                    else
                    {
                        return null;
                    }
            }
        }

        public static byte[] StringToBytes(string str)
        {
            if (str == null)
                str = string.Empty;

            return ByteUtil.Xor(StringEncoding.GetBytes(str), StringXorKey);
        }

        public static string BytesToString(byte[] b)
        {
            return StringEncoding.GetString(ByteUtil.Xor(b, StringXorKey));
        }

        public static byte[] SingleObject(object obj, out byte typeCode)
        {
            typeCode = TypeToCode(obj?.GetType());

            switch (typeCode)
            {
                // Null value
                case 1:
                    return nullBytes;

                case 2:
                    return new byte[] { (byte)obj };
                case 3:
                    return BitConverter.GetBytes((char)obj);
                case 4:
                    return BitConverter.GetBytes((bool)obj);
                case 5:
                    return BitConverter.GetBytes((Int16)obj);
                case 6:
                    return BitConverter.GetBytes((Int32)obj);
                case 7:
                    return BitConverter.GetBytes((Int64)obj);
                case 8:
                    return StringToBytes((string)obj);
                case 9:
                    return BitConverter.GetBytes((double)obj);
                case 10:
                    return BitConverter.GetBytes((float)obj);
                case 11:
                    return BitConverter.GetBytes((UInt16)obj);
                case 12:
                    return BitConverter.GetBytes((UInt32)obj);
                case 13:
                    return BitConverter.GetBytes((UInt64)obj);
                /*case 14:
                    return BitConverter.GetBytes((Int32)obj);*/
                case 15:
                    return ArrayToBytes((Array)obj);
                case 16:
                    return BitConverter.GetBytes(((DateTime)obj).Ticks);
                case 17:
                    return BitConverter.GetBytes(((TimeSpan)obj).Ticks);

                default:
                    if (CustomTypes.ContainsKey(typeCode))
                        return Bytes(obj);

                    return new byte[] { 0x0 };
            }
        }

        public static Type CodeToType(byte typeCode)
        {
            switch(typeCode)
            {
                case 0:
                    return typeof(object);

                case 1:
                    return null;

                case 2:
                    return typeof(byte);
                case 3:
                    return typeof(char);
                case 4:
                    return typeof(bool);
                case 5:
                    return typeof(Int16);
                case 6:
                    return typeof(Int32);
                case 7:
                    return typeof(Int64);
                case 8:
                    return typeof(string);
                case 9:
                    return typeof(double);
                case 10:
                    return typeof(float);
                case 11:
                    return typeof(UInt16);
                case 12:
                    return typeof(UInt32);
                case 13:
                    return typeof(UInt64);
                /*case 14:
                    return typeof(Int32); */// Enum as Int32
                case 15:
                    return typeof(Array);
                case 16:
                    return typeof(DateTime);
                case 17:
                    return typeof(TimeSpan);

                default:
                    if (CustomTypes.ContainsKey(typeCode))
                        return CustomTypes[typeCode];
                    else
                        return null;
            }
        }

        public static byte TypeToCode(Type t)
        {
            if (t == null)
                return 1;

            if (t == typeof(byte))
                return 2;
            else if (t == typeof(char))
                return 3;
            else if (t == typeof(bool))
                return 4;
            else if (t == typeof(Int16))
                return 5;
            else if (t == typeof(Int32))
                return 6;
            else if (t == typeof(Int64))
                return 7;
            else if (t == typeof(string))
                return 8;
            else if (t == typeof(double))
                return 9;
            else if (t == typeof(float))
                return 10;
            else if (t == typeof(UInt16))
                return 11;
            else if (t == typeof(UInt32))
                return 12;
            else if (t == typeof(UInt64))
                return 13;
            /*else if (t.IsEnum)
                return 14;*/
            else if (t.IsArray)
                return 15;
            else if (t == typeof(DateTime))
                return 16;
            else if (t == typeof(TimeSpan))
                return 17;
            /*else if (t == typeof(object))
                return 16;*/

            else if (CustomTypes.Count((e) => e.Value == t) > 0)//e.Value.IsAssignableFrom(t)//CustomTypes.ContainsValue(t)
                return CustomTypes.First((e) => e.Value == t).Key;
            else
                return 0;
        }

        public static byte[] ArrayToBytes(Array a, bool forceMultiType = false)
        {
            // The type-code of the current item being red.
            byte vtc = 0;
            Stack<byte[]> buffers = new Stack<byte[]>();
            long total = 0;

            byte ptc = 0;
            bool multiType = forceMultiType;

            int i = 0;
            foreach(var v in a)
            {
                byte[] buffer = SingleObject(a.GetValue(i), out vtc);
                byte[] tbuffer = new byte[buffer.Length + 5];

                if (i == 0)
                    ptc = vtc;

                if (vtc != ptc)
                    multiType = true;

                tbuffer[0] = vtc;
                Array.Copy(BitConverter.GetBytes(buffer.Length), 0, tbuffer, 1, 4);
                Array.Copy(buffer, 0, tbuffer, 5, buffer.Length);
                buffers.Push(tbuffer);
                total += tbuffer.Length;

                i++;
            }

            buffers.Push(BitConverter.GetBytes(a.Length));
            buffers.Push(new byte[] { (byte)(multiType ? 0 : vtc) });
            total += 5;

            return CombineStack(buffers, total);
        }

        public static object AnyBytesToArray(byte[] b)
        {
            byte t = b[0];

            MethodInfo method = typeof(Bytifier).GetMethod(nameof(BytesToArray));
            MethodInfo generic = method.MakeGenericMethod(CodeToType(t));
            return generic.Invoke(null, new object[] { b });
        }

        public static TArray[] BytesToArray<TArray>(byte[] b)
        {
            int length = BitConverter.ToInt32(b, 1);
            Type t = typeof(TArray);
            TArray[] arr = new TArray[length];

            byte ctc = 0;
            bool doNew = true;
            int len = 0;
            int c = 0;
            byte[] buffer = new byte[1];
            bool doRead = false;
            int index = length - 1;

            for (int i = 5; i < b.Length; i++)
            {
                byte red = b[i];

                if (doRead)
                {
                    buffer[c] = red;

                    c++;

                    if (c == len)
                    {
                        object o = SingleBuffer(buffer, ctc);
                        
                        //Console.WriteLine($"Array item ({ o?.GetType() }) => " + o);

                        arr[index] = (TArray)o;
                        index--;

                        doRead = false;
                        doNew = true;
                        continue;
                    }
                }

                if (doNew)
                {
                    doNew = false;
                    doRead = true;
                    ctc = red;
                    len = BitConverter.ToInt32(b, i + 1);
                    buffer = new byte[len];
                    c = 0;
                    i += 4;

                   // Console.WriteLine($"New reader array reader with length { len } for object type { ctc }");

                    continue;
                }
            }

            return arr;
        }

    }
}
