using System.IO;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;

namespace MechabellumModding
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInProcess("Mechabellum.exe")]
    public class MechabellumModding : BasePlugin
    {
        internal static new ManualLogSource Log;

        public override void Load()
        {
            /* Setup Logging */
            MechabellumModding.Log = base.Log;
            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

            /* Check for early exit due to disable switch */
            ModConfig.Load();
            if (ModConfig.data.disableAllMods)
            {
                MechabellumModding.Log.LogInfo($"All mods DISABLED!");
                return;
            }

            /* Inject Mods */
        }
    }
}
