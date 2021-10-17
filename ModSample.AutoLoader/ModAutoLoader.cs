using System.IO;
using UnityModLoader.Library.Core.Logging;
using UnityModLoader.Library.Mods;

namespace ModSample.AutoLoader
{
    public class ModAutoLoader : UnityMod
    {
        public override string Name => "Mod Auto Loader";
        public override string Description => "Shows a confirmation dialog when new mod is detected";
        public override string Author => "NatanM";
        public override string Version => "1.0";

        public override void Init()
        {
            UnityEngine.GameObject holder = new UnityEngine.GameObject("ModLoaderUI");
            UnityEngine.GameObject.DontDestroyOnLoad(holder);
            LoaderUI ui = holder.AddComponent<LoaderUI>();

            FileSystemWatcher watcher = new FileSystemWatcher("./Mods", "*.dll");
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Created += (s, e) =>
            {
                Logger.Instance.Log($"New mod file detected, {e.Name}");
                ui.Files.Add(new FileInfo(e.FullPath));
            };
            watcher.EnableRaisingEvents = true;
        }
    }
}
