using System.IO;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;

namespace MechabellumModding
{
    public static class ModConfig
    {
        private static ConfigFile configFile;
        private static ConfigEntry<bool> configDisableAllMods;
        public static Data data = new Data();
        
        public class Data
        {
            public bool disableAllMods;
        }

        public static void Load()
        {
            /* Initialize Configuration */
            configFile = new ConfigFile(Path.Combine(Paths.ConfigPath, "mechabellum-modding.cfg"), true);
            configDisableAllMods = configFile.Bind(
                "General",
                "DisableAllMods",
                false,
                "Set to true to completely return the game back to it's original state"
            );
            data.disableAllMods = configDisableAllMods.Value;
        }
    }
}
