using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityModLoader.Library.Core;

namespace ModSample.AutoLoader
{
    public class LoaderUI : MonoBehaviour
    {
        public List<FileInfo> Files = new List<FileInfo>();

        Rect windowRect = new Rect(0, 0, 500, 70);
        Rect nButton = new Rect(5, 20, 100, 45);
        Rect yButton = new Rect(500 - 105, 20, 100, 45);
        private void OnGUI()
        {
            if (Files.Count == 0)
                return;

            GUI.Window(0, windowRect, ExecWindow, $"Do you want to load \"{Files[0].Name}\"");
        }

        void ExecWindow(int id)
        {
            if (GUI.Button(nButton, "No"))
            {
                UnityModLoader.Library.Core.Logging.Logger.Instance.Log($"Mod \"{Files[0].Name}\" ignored by user.");

                Files[0].MoveTo(Path.Combine(Files[0].Directory.FullName, $"{Files[0].Name}.disabled"));
                Files.RemoveAt(0);
                return;
            }

            if (GUI.Button(yButton, "Yes"))
            {

                try
                {
                    ModLoader.Instance.LoadModAssembly(Files[0].FullName);
                    UnityModLoader.Library.Core.Logging.Logger.Instance.Log($"Mod \"{Files[0].Name}\" loaded by user.");
                }
                catch (Exception ex)
                {
                    UnityModLoader.Library.Core.Logging.Logger.Instance.Log($"Could not load \"{Files[0].Name}\". {ex.Message}", messageType: UnityModLoader.Library.Core.Logging.Logger.MessageType.Warning);
                    Files[0].MoveTo(Path.Combine(Files[0].Directory.FullName, $"{Files[0].Name}.disabled"));
                }

                Files.RemoveAt(0);
                return;
            }
        }
    }
}
