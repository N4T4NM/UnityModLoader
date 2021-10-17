using System;
using System.Reflection;

namespace UnityModLoader.Library.Core.Exceptions
{
    public class InvalidAssemblyException : Exception
    {
        public InvalidAssemblyException(Assembly asm) : base($"Assembly: {asm.GetName().Name}\nNot a valid unity mod.") { }
    }
}
