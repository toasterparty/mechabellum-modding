using System;
using System.IO;
using System.Reflection;

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
    }
}
