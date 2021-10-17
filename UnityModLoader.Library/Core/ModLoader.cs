using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityModLoader.Library.Core.Logging;
using UnityModLoader.Library.Mods;
using UnityModLoader.Library.Mods.Utils;

namespace UnityModLoader.Library.Core
{
    public class ModLoader
    {
        public static readonly ModLoader Instance = new ModLoader();

        bool GetSymbolStore(FileInfo dllFile, out byte[] data)
        {
            data = null;
            string pdbName = dllFile.Name.Remove(dllFile.Name.LastIndexOf('.')) + ".pdb";

            FileInfo pdbFile = dllFile.Directory.GetFiles(pdbName).FirstOrDefault();
            if (pdbFile == null)
                return false;

            data = File.ReadAllBytes(pdbFile.FullName);
            return true;
        }

        public void LoadModAssembly(FileInfo modFile)
        {
            Assembly asm;
            if (GetSymbolStore(modFile, out byte[] symbols))
                asm = Assembly.Load(File.ReadAllBytes(modFile.FullName), symbols);
            else asm = Assembly.Load(File.ReadAllBytes(modFile.FullName));

            IUnityMod mod = ModAssemblyUtility.GetMod(asm);
            mod.Init();
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
                    LoadModAssembly(mod);
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
            FileInfo dependencyFile = new FileInfo($"./Dependencies/{dll}");

            if (dependencyFile.Exists)
            {
                if (GetSymbolStore(dependencyFile, out byte[] symbols))
                    return Assembly.Load(File.ReadAllBytes(dependencyFile.FullName), symbols);
                else return Assembly.Load(File.ReadAllBytes(dependencyFile.FullName));
            }

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