using System;
using System.Reflection;
using UnityModLoader.Library.Core.Exceptions;

namespace UnityModLoader.Library.Mods.Utils
{
    public static class ModAssemblyUtility
    {
        public static IUnityMod GetMod(Assembly modAsm)
        {
            foreach (Type type in modAsm.GetTypes())
                if (type.BaseType == typeof(IUnityMod))
                    return (IUnityMod)modAsm.CreateInstance(type.FullName);

            throw new InvalidAssemblyException(modAsm);
        }
    }
}
