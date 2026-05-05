using HarmonyLib;
using HermitMod.Utility;
using MegaCrit.Sts2.Core.Hooks;

namespace HermitMod.Patches;

/// <summary>
/// Resets the Dead On trigger counter at the start of each turn.
/// </summary>
[HarmonyPatch(typeof(Hook), "BeforeSideTurnStart")]
public static class DeadOnTurnResetPatch
{
    [HarmonyPrefix]
    public static void Prefix()
    {
        DeadOnCounter.Reset();
    }
}
