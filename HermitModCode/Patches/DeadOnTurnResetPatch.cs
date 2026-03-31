using HarmonyLib;
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
        DeadOnHelper.ResetTurnCount();
    }
}
