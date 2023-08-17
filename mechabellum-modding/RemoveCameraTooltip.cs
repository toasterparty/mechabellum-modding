using HarmonyLib;

using GameRiver.Client;

namespace MechabellumModding
{
    public static class RemoveCameraTooltip
    {
        public static void Load()
        {            
            if (!ModConfig.Data.RemoveCameraTooltip)
            {
                return;
            }

            Harmony.CreateAndPatchAll(typeof(RemoveCameraTooltip));
        }

        [HarmonyPatch(typeof(CameraTips), nameof(CameraTips.ShowTips))]
        [HarmonyPrefix]
        private static bool ShowTips(ref CameraTips.TipType tipType, ref CameraTips __instance)
        {
            foreach (var tip in __instance.tips)
            {
                tip.active = false;
            }
            return false;
        }
    }
}
