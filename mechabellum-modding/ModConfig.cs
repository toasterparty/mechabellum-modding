using System.IO;

using BepInEx;
using BepInEx.Configuration;

namespace MechabellumModding
{
    public static class ModConfig
    {
        private static ConfigFile configFile;
        private static readonly ConfigData data = new();

        public class ConfigData
        {
            public bool disableAllMods;
            public bool customRecommendedFormations;
        }

        public static ConfigData Data
        {
            get
            {
                return data;
            }
        }

        public static void Load()
        {
            /* Initialize Configuration */
            configFile = new ConfigFile(Path.Combine(Paths.ConfigPath, "mechabellum-modding.cfg"), true);

            /* Read values */
            data.disableAllMods = configFile.Bind(
                "General",
                "DisableAllMods",
                false,
                "Set to true to completely return the game back to it's original state"
            ).Value;

            data.customRecommendedFormations = configFile.Bind(
                "General",
                "CustomRecommendedFormations",
                false,
                "Replaces the community-provided Recommended starting formations with ones you create"
            ).Value;

        }
    }
}
