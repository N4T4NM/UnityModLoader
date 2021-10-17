using System;
namespace UnityModLoader.Library.Mods
{
    public abstract class UnityMod : MarshalByRefObject
    {
        public virtual string Name { get; }
        public virtual string Description { get; }
        public virtual string Version { get; }
        public virtual string Author { get; }

        /// <summary>
        /// Called after injection
        /// </summary>
        public virtual void Init() { }
    }
}
