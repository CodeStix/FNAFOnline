using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Stx.Utilities;

namespace Stx.Serialization
{
    public static class Bytifile
    {
        public static T FromFile<T>(string path) where T : IByteDefined, new()
        {
            try
            {
                return Bytifier.Object<T>(File.ReadAllBytes(path));
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public static void ToFile<T>(string path, T obj) where T : IByteDefined, new()
        {
            try
            {
                File.WriteAllBytes(path, Bytifier.Bytes(obj));
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
