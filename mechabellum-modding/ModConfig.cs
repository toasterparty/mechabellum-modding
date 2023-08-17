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
            public bool CustomRecommendedFormationKeyboardShortcuts;
            public bool RemoveCameraTooltip;
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
                "_General_",
                "DisableAllMods",
                false,
                "Set to true to completely return the game back to it's original state. Supersedes all other options"
            ).Value;

            data.RemoveCameraTooltip = configFile.Bind(
                "_General_",
                "RemoveCameraTooltip",
                false,
                "Hides the tutorial text that is otherwise permanently in the bottom left corner of the match screen"
            ).Value;

            data.customRecommendedFormations = configFile.Bind(
                "CustomFormations",
                "Enabled",
                false,
                "Replaces the community-provided recommended starting formations with ones you create"
            ).Value;

            data.CustomRecommendedFormationKeyboardShortcuts = configFile.Bind(
                "CustomFormations",
                "KeyboardShortcuts",
                false,
                "If custom starting formations are enabled, this setting allows you to browse and create formations with keyboard shortcuts. They are hard-coded as follows:\n - Page Up: Next Formation\n - Page Down: Previous Formation\n - Insert: Add Formation\n - Delete: Remove Formation"
            ).Value;
        }
    }
}
