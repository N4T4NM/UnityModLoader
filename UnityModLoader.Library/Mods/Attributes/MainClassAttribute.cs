using System;

namespace UnityModLoader.Library.Mods.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MainClassAttribute : Attribute
    {
        public string ModName { get; set; }
        public string ModDescription { get; set; }
        public string ModAuthor { get; set; }
    }
}
