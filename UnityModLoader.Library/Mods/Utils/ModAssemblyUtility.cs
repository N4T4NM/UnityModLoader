using System;
using System.Reflection;
using UnityModLoader.Library.Core.Exceptions;
using UnityModLoader.Library.Mods.Attributes;

namespace UnityModLoader.Library.Mods.Utils
{
    public static class ModAssemblyUtility
    {
        public static Type GetMainClass(Assembly asm)
        {
            foreach (Type type in asm.GetTypes())
                if (type.GetCustomAttributes(typeof(MainClassAttribute), true)?.Length > 0)
                    return type;

            throw new InvalidAssemblyException(asm);
        }
        public static MethodInfo GetEntryPoint(Type mainClass)
        {
            foreach (MethodInfo method in mainClass.GetMethods())
                if (method.GetCustomAttributes(typeof(EntryPointAttribute), true)?.Length > 0)
                    return method;

            throw new InvalidClassException(mainClass);
        }
    }
}
