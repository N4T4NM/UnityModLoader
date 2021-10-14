using System;

namespace UnityModLoader.Library.Core.Exceptions
{
    public class InvalidClassException : Exception
    {
        public InvalidClassException(Type mainClass) : base($"Main Class: {mainClass.Name}\n\"EntryPointAttribute\" not found.") { }
    }
}
