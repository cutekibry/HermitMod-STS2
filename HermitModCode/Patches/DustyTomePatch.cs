using HarmonyLib;
using HermitMod.Character;
using MegaCrit.Sts2.Core.Models.Relics;

namespace HermitMod.Patches;

/// <summary>
/// Prevents NullReferenceException in DustyTome.SetupForPlayer when
/// the player is using a custom character (The Hermit) that may not have
/// all the properties the relic expects.
/// </summary>
[HarmonyPatch(typeof(DustyTome), "SetupForPlayer")]
public static class DustyTomePatch
{
    [HarmonyPrefix]
    public static bool Prefix(DustyTome __instance)
    {
        try
        {
            // Check if the owner is The Hermit — if so, skip the setup
            // to prevent NRE from missing character-specific properties
            if (__instance.Owner?.Character is Hermit)
                return false; // Skip original method

            return true; // Run original method for base game characters
        }
        catch
        {
            return false; // If anything goes wrong, skip to prevent crash
        }
    }
}
