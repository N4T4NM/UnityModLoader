using System;
using System.IO;
using System.Reflection;
using UnityModLoader.Library.Core.Exceptions;
using UnityModLoader.Library.Core.Logging;
using UnityModLoader.Library.Mods;
using UnityModLoader.Library.Mods.Attributes;

namespace UnityModLoader.Core
{
    public class ModLoader
    {
        public static readonly ModLoader Instance = new ModLoader();

        public Type GetMainClass(Assembly asm)
        {
            foreach (Type type in asm.GetTypes())
                if (type.GetCustomAttributes(typeof(MainClassAttribute), true)?.Length > 0)
                    return type;

            throw new InvalidAssemblyException(asm);
        }
        public MethodInfo GetEntryPoint(Type mainClass)
        {
            foreach (MethodInfo method in mainClass.GetMethods())
                if (method.GetCustomAttributes(typeof(EntryPointAttribute), true)?.Length > 0)
                    return method;

            throw new InvalidClassException(mainClass);
        }

        public void LoadDependencyAssembly(string path)
            => Assembly.Load(File.ReadAllBytes(path));
        public void LoadModAssembly(string path)
        {
            Assembly asm = Assembly.Load(File.ReadAllBytes(path));

            Type mainClass = GetMainClass(asm);
            MethodInfo entryPoint = GetEntryPoint(mainClass);

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
                    Logger.Instance.Append($"ERROR {ex.Message}\n\n");
                }
            }

            Logger.Instance.Append("\n");
            Logger.Instance.Log("== Mods Loaded ==\n");
        }
        void LoadDependencies()
        {
            DirectoryInfo depsDir = new DirectoryInfo("Dependencies");
            if (!depsDir.Exists)
            {
                Logger.Instance.Log("\"Dependencies\" folder not found.", messageType: Logger.MessageType.Warning);
                depsDir.Create();
                return;
            }

            Logger.Instance.Log("== Loading Dependencies ==\n");
            foreach (FileInfo dependency in depsDir.GetFiles("*.dll"))
            {
                Logger.Instance.Log($"Loading \"{dependency.Name}\"... ", false);
                try
                {
                    LoadDependencyAssembly(dependency.FullName);
                    Logger.Instance.Append("LOADED\n");
                }
                catch (Exception ex)
                {
                    Logger.Instance.Append($"ERROR {ex.Message}\n\n");
                }
            }

            Logger.Instance.Append("\n");
            Logger.Instance.Log("== Dependecies Loaded ==\n");
        }

        void Load()
        {
            LoadDependencies();
            LoadMods();
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