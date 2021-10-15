using System.IO;
using UnityModLoader.Library.Core.Logging;
using UnityModLoader.Library.Mods;
using UnityModLoader.Library.Mods.Attributes;

namespace ModSample.AutoLoader
{
    [MainClass(
        ModAuthor = "NatanM",
        ModDescription = "Shows a confirmation dialog when new mod is detected",
        ModName = "Auto Loader"
        )]
    public class ModAutoLoader
    {
        [EntryPoint]
        public static void Run()
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
