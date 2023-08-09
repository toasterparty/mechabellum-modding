using BepInEx;
using BepInEx.Unity.IL2CPP;

namespace mechabellum_modding;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("Mechabellum.exe")]
public class Plugin : BasePlugin
{
    public override void Load()
    {
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }
}
