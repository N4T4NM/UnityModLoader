namespace UnityModLoader.Library.Mods
{
    public interface IUnityMod
    {
        string Name { get; }
        string Description { get; }
        string Version { get; }
        string Author { get; }

        /// <summary>
        /// Called after injection
        /// </summary>
        void Init();
    }
}
