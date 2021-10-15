using System;
using System.IO;
using System.Reflection;
using UnityModLoader.Library.Core.Logging;
using UnityModLoader.Library.Mods.Utils;

namespace UnityModLoader.Library.Core
{
    public class ModLoader
    {
        public static readonly ModLoader Instance = new ModLoader();

        public void LoadModAssembly(string path)
        {
            Assembly asm = Assembly.Load(File.ReadAllBytes(path));

            Type mainClass = ModAssemblyUtility.GetMainClass(asm);
            MethodInfo entryPoint = ModAssemblyUtility.GetEntryPoint(mainClass);

            entryPoint.Invoke(null, null);
        }

        void LoadMods()
        {
            DirectoryInfo modsDir = new DirectoryInfo("Mods");

            if (!modsDir.Exists)
            {
                Logger.Instance.Log("\"Mods\" folder not found.", messageType: Logger.MessageType.Warning);
                modsDir.Create();
                return;
            }

            Logger.Instance.Log("== Loading Mods ==\n");

            foreach (FileInfo mod in modsDir.GetFiles("*.dll"))
            {
                Logger.Instance.Log($"Loading \"{mod.Name}\"... ", false);
                try
                {
                    LoadModAssembly(mod.FullName);
                    Logger.Instance.Append("LOADED\n");
                }
                catch (Exception ex)
                {
                    Logger.Instance.Append(ex.ToString());
                }
            }

            Logger.Instance.Append("\n");
            Logger.Instance.Log("== Mods Loaded ==\n");
        }

        void Load()
        {
            AppDomain.CurrentDomain.AssemblyResolve += DynamicAssemblyLoad;
            LoadMods();
        }

        private Assembly DynamicAssemblyLoad(object sender, ResolveEventArgs args)
        {
            string dll = $"{args.Name.Split(',')[0]}.dll";
            string dependencyPath = $"./Dependencies/{dll}";

            if (File.Exists(dependencyPath))
                return Assembly.Load(File.ReadAllBytes(dependencyPath));

            return null;
        }

        public static void StartModLoader()
        {
            try
            {
                Logger.Instance.Log("UnityModLoader Started.");
                Instance.Load();
            }
            catch (Exception ex)
            {
                Logger.Instance.Log("\n== UNHANDLED EXCEPTION ==\n", messageType: Logger.MessageType.Error);
                Logger.Instance.Append(ex.ToString());
                Environment.Exit(-1);
            }
        }
    }
}