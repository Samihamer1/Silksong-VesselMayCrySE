using BepInEx;
using HarmonyLib;
using System;

namespace VesselMayCrySE;

// TODO - adjust the plugin guid as needed
[BepInDependency("org.silksong-modding.fsmutil")]
[BepInDependency("org.silksong-modding.i18n")]
[BepInAutoPlugin(id: "io.github.samihamer1.vesselmaycryse")]
public partial class VesselMayCrySEPlugin : BaseUnityPlugin
{
    internal static VesselMayCrySEPlugin Instance = null!;
    private Harmony harmony = null!;
    private void Awake()
    {
        Instance = this;
        ResourceLoader.Init();

        Logger.LogInfo($"Plugin {Name} ({Id}) has loaded!");

        harmony = new Harmony($"harmony-auto-{(object)Guid.NewGuid()}");
        harmony.PatchAll(typeof(Patches));
    }

    public void log(string message)
    {
        Logger.LogInfo(message);
    }

    public void LogError(string message)
    {
        Logger.LogError(message);
    }
}
