using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Stx.Utilities
{
    /// <summary>
    /// A mush faster alternative to <see cref="Activator"/>. 
    /// But only supports parameterless constructors.
    /// </summary>
    public static class FastActivator
    {
        private delegate object ObjectActivator();

        public static object CreateInstance(Type type)
        {
            if (type == null)
                throw new NullReferenceException("Type cannot be null.");

            ConstructorInfo emptyConstructor = type.GetConstructor(Type.EmptyTypes);
            var dynamicMethod = new DynamicMethod("CreateInstance", type, Type.EmptyTypes, true);
            ILGenerator ilGenerator = dynamicMethod.GetILGenerator();
            ilGenerator.Emit(OpCodes.Nop);
            ilGenerator.Emit(OpCodes.Newobj, emptyConstructor);
            ilGenerator.Emit(OpCodes.Ret);
            var ac = (ObjectActivator)dynamicMethod.CreateDelegate(typeof(ObjectActivator));
            return ac.Invoke();
        }

        /*public static object InvokeGenericMethod(Type methodContaining, string methodName, object instance, Type generic1, params object[] param)
        {
            MethodInfo method = methodContaining.GetMethod(methodName);
            MethodInfo generic = method.MakeGenericMethod(generic1);
            return generic.Invoke(null, param);
        }*/
    }
}
