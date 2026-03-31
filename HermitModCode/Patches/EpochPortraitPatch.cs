using HarmonyLib;
using MegaCrit.Sts2.Core.Timeline;

namespace HermitMod.Patches;

/// <summary>
/// Patches the epoch portrait loading to provide custom images for Hermit epochs.
/// The base game loads portraits from paths inside the game's .pck file.
/// Since mods can't inject into that path, we redirect to our mod's image directory.
/// </summary>
[HarmonyPatch]
public static class EpochPortraitPatch
{
    private const string HermitPrefix = "HERMITMOD-";
    private const string EpochImageDir = "res://HermitMod/images/epochs/";

    [HarmonyPatch(typeof(EpochModel), nameof(EpochModel.BigPortraitPath), MethodType.Getter)]
    [HarmonyPostfix]
    public static void BigPortraitPathPostfix(EpochModel __instance, ref string __result)
    {
        if (__instance.Id.StartsWith(HermitPrefix))
        {
            __result = EpochImageDir + __instance.Id.ToLowerInvariant() + ".png";
        }
    }

    [HarmonyPatch(typeof(EpochModel), nameof(EpochModel.PackedPortraitPath), MethodType.Getter)]
    [HarmonyPostfix]
    public static void PackedPortraitPathPostfix(EpochModel __instance, ref string __result)
    {
        if (__instance.Id.StartsWith(HermitPrefix))
        {
            __result = EpochImageDir + __instance.Id.ToLowerInvariant() + "_small.png";
        }
    }
}
