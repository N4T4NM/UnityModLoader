using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using UnityModLoader.Library.Mods.Attributes;
using UnityModLoader.Library.Mods.Utils;

namespace UnityModLoader.Manager
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        FileInfo currentGame;
        DirectoryInfo managedDir;

        void SelectGameExecutable()
        {
            if (MessageBox.Show("Could not find a unity game. Do you want to select the game executable ?",
                "Game not found",
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Multiselect = false;
                dialog.Filter = "Executable Files|*.exe";
                if (dialog.ShowDialog() == true)
                {
                    currentGame = new FileInfo(dialog.FileName);
                    return;
                }
            }
            Environment.Exit(-1);
        }
        bool FindGame()
        {
            DirectoryInfo dir = new DirectoryInfo(Environment.CurrentDirectory);
            DirectoryInfo gameData = dir.GetDirectories("*_Data").FirstOrDefault();

            if (gameData == null)
            {
                SelectGameExecutable();
                Environment.CurrentDirectory = currentGame.Directory.FullName;
                return FindGame();
            }

            currentGame = new FileInfo($"{ gameData.Name.Split('_')[0] }.exe");
            managedDir = new DirectoryInfo(Path.Combine(gameData.FullName, "Managed"));
            if (!currentGame.Exists)
            {
                SelectGameExecutable();
                return false;
            }

            return true;
        }

        void AddMod(FileInfo modFile)
        {
            Assembly modAsm = Assembly.Load(File.ReadAllBytes(modFile.FullName));

            MainClassAttribute main = ModAssemblyUtility.GetMainClass(modAsm)
                .GetCustomAttribute<MainClassAttribute>();

            ModControl control = new ModControl();
            control.ModName = main.ModName;
            control.ModDescription = main.ModDescription;
            control.ModAuthor = main.ModAuthor;

            control.ModPath = modFile;
            control.EnabledCheck.IsChecked = modFile.Name.EndsWith(".dll");

            ModsData.Children.Add(control);
        }
        void RemoveMod(FileInfo modFile)
        {
            ModControl mod = FindMod(modFile);
            if(mod != null)
                ModsData.Children.Remove(mod);
        }
        ModControl FindMod(FileInfo modFile)
        {
            foreach(UIElement element in ModsData.Children)
            {
                ModControl mod = element as ModControl;
                if (mod.ModPath.FullName == modFile.FullName)
                    return mod;
            }
            return null;
        }
        void FindMods()
        {
            DirectoryInfo modsDir = new DirectoryInfo("./Mods");
            if (!modsDir.Exists)
                modsDir.Create();

            Dictionary<FileInfo, string> errors = new Dictionary<FileInfo, string>();
            foreach (FileInfo modFile in modsDir.GetFiles("*.dll*"))
            {
                if (!modFile.Name.EndsWith(".dll") && !modFile.Name.EndsWith(".dll.disabled"))
                    continue;

                try
                {
                    AddMod(modFile);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    if (ex is ReflectionTypeLoadException)
                    {
                        Exception[] loader = (ex as ReflectionTypeLoadException).LoaderExceptions;
                        string err = string.Join("\n", loader.Select(e => e.Message));

                        errors.Add(modFile, err);
                    }
                    else
                        errors.Add(modFile, ex.Message);
                }
            }

            if (errors.Count > 0)
            {
                string errorsStr = string.Join("\n\n", errors.Select((k) =>
                {
                    return $"{k.Key}: {k.Value}";
                }));

                MessageBox.Show($"Could not load some mods.\n\n{errorsStr}",
                    "Errors", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!FindGame())
                return;
            CurrentGame.Text = $"Game: {currentGame.Name.Remove(currentGame.Name.LastIndexOf('.'))}";

            AppDomain.CurrentDomain.AssemblyResolve += ResolveDependency;
            FindMods();
            ManageFileSystem();
        }
        void ManageFileSystem()
        {
            FileSystemWatcher fs = new FileSystemWatcher("./Mods", "*.dll");
            fs.Created += (s, ev) =>
            {
                this.Dispatcher.Invoke(()=>AddMod(new FileInfo(ev.FullPath)));
            };
            fs.Deleted += (s, ev) =>
            {
                this.Dispatcher.Invoke(() => RemoveMod(new FileInfo(ev.FullPath)));
            };
            fs.Renamed += (s, ev) =>
            {
                if (ev.Name.EndsWith(".dll.disabled"))
                    return;
                this.Dispatcher.Invoke(() =>
                {
                    ModControl mod = FindMod(new FileInfo(ev.OldFullPath));
                    if (mod != null)
                        mod.ModPath = new FileInfo(ev.FullPath);
                });
            };

            fs.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName;
            fs.EnableRaisingEvents = true;
        }

        private Assembly ResolveDependency(object sender, ResolveEventArgs args)
        {
            string dllName = $"{args.Name.Split(',')[0]}.dll";
            string targetPath = Path.Combine(managedDir.FullName, dllName);
            if (File.Exists(targetPath))
                return Assembly.Load(File.ReadAllBytes(targetPath));
            else if (File.Exists(Path.Combine(Environment.CurrentDirectory, "Dependencies", dllName)))
                return Assembly.Load(File.ReadAllBytes(Path.Combine(Environment.CurrentDirectory, "Dependencies", dllName)));

            return null;
        }

        private void LaunchButton_Click(object sender, RoutedEventArgs e)
            => new Injector().Inject(currentGame);
    }
}
