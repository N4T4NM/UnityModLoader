using System;

namespace UnityModLoader.Library.Mods
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class EntryPointAttribute : Attribute { }
}
