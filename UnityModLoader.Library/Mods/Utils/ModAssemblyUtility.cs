using System;
using System.Diagnostics;
using System.Reflection;
using UnityModLoader.Library.Core.Exceptions;

namespace UnityModLoader.Library.Mods.Utils
{
    public static class ModAssemblyUtility
    {
        public static IUnityMod GetMod(Assembly modAsm)
        {
            foreach (Type type in modAsm.GetTypes())
            {
                Debug.WriteLine(type.FullName);
                if (typeof(IUnityMod).IsAssignableFrom(type))
                    return (IUnityMod)modAsm.CreateInstance(type.FullName);
            }

            throw new InvalidAssemblyException(modAsm);
        }
    }
}
