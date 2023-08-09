using System;

using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;

using HarmonyLib;
using UnityEngine;

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
            Log = base.Log;
            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

            /* Check for early exit due to disable switch */
            ModConfig.Load();
            if (ModConfig.data.disableAllMods)
            {
                Log.LogInfo($"All mods DISABLED!");
                return;
            }

            /* Inject a custom monobehavior so we can operate neatly within the engine */
            AddComponent<MechabellumModdingMB>();

            /* Inject Mods */
            Harmony.CreateAndPatchAll(typeof(MechabellumModding));
            ReccomendedFormations.Load();
        }
    }

    public class MechabellumModdingMB : MonoBehaviour
    {
        public MechabellumModdingMB(IntPtr handle) : base(handle) {}
        #pragma warning disable IDE0051
        #pragma warning disable CA1822
        private void Update()
        {
            
        }
    }
}
