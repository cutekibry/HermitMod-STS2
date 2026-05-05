using HarmonyLib;
using HermitMod.Cards;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Patches;

[HarmonyPatch(typeof(CardModel), nameof(CardModel.OnPlayWrapper))]
public static class DeadOnPlayCapturePatch
{
    [HarmonyPrefix]
    public static void Prefix(CardModel __instance)
    {
        if (__instance is HermitCard hermitCard) {
            Log.Info($"Card is a HermitCard, capturing Dead On for play");
            hermitCard.CaptureDeadOnForPlay();
        }
    }
}
