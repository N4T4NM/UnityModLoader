using System;
using System.Reflection;
using UnityModLoader.Library.Core.Exceptions;

namespace UnityModLoader.Library.Mods.Utils
{
    public static class ModAssemblyUtility
    {
        public static UnityMod GetMod(Assembly modAsm)
        {
            foreach (Type type in modAsm.GetTypes())
                if (type.BaseType == typeof(UnityMod))
                    return (UnityMod)modAsm.CreateInstance(type.FullName);

            throw new InvalidAssemblyException(modAsm);
        }
    }
}
