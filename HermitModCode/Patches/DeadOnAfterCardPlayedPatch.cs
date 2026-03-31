using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;

namespace HermitMod.Patches;

/// <summary>
/// Resets the Dead On flag after a card play has fully resolved.
/// </summary>
[HarmonyPatch(typeof(Hook), "AfterCardPlayed")]
public static class DeadOnAfterCardPlayedPatch
{
    [HarmonyPostfix]
    public static void Postfix(CombatState combatState, PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        DeadOnHelper.SetDeadOn(value: false);
    }
}
