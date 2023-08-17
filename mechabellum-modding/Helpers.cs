using System;
using System.IO;
using System.Reflection;

using UnityEngine;
using GameRiver.Client;

namespace MechabellumModding
{
    public static class Helpers
    {
        public static string GameFolderPath
        {
            get
            {
                var filepath = Assembly.GetExecutingAssembly().Location;
                UriBuilder uri = new(filepath);
                var assemblyPath = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(assemblyPath + "\\..\\..\\..\\..");
            }
        }

        public static MainUI MainUI
        {
            get
            {
                var uis = UnityEngine.Object.FindObjectsOfType<MainUI>();
                foreach (var ui in uis) 
                {
                    if (ui.match != null)
                    {
                        return ui;
                    }
                }

                return null;
            }
        }
    }
}
