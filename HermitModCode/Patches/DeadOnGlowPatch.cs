using System.Collections.Generic;
using HarmonyLib;
using HermitMod.Cards;
using HermitMod.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Patches;

/// <summary>
/// Makes cards with Dead On effects glow gold when they are in the Dead On position
/// (middle of the hand) OR when the player has Concentration active.
/// </summary>
[HarmonyPatch(typeof(CardModel), nameof(CardModel.ShouldGlowGold), MethodType.Getter)]
public static class DeadOnGlowPatch
{
    [HarmonyPostfix]
    public static void Postfix(CardModel __instance, ref bool __result)
    {
        if (__result || __instance is not HermitCard { HasDeadOn: not false })
            return;

        CardPile pile = __instance.Pile;
        if (pile == null || pile.Type != PileType.Hand)
            return;

        IReadOnlyList<CardModel> cards = pile.Cards;
        if (cards == null || cards.Count == 0)
            return;

        Player owner = __instance.Owner;
        Creature creature = owner?.Creature;

        if (creature?.Powers != null)
        {
            foreach (PowerModel power in creature.Powers)
            {
                if (power is ConcentrationPower concentrationPower && concentrationPower.Amount > 0)
                {
                    __result = true;
                    return;
                }
            }
        }

        int count = cards.Count;
        int cardIndex = -1;
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i] == __instance)
            {
                cardIndex = i;
                break;
            }
        }

        if (cardIndex >= 0)
        {
            __result = DeadOnHelper.IsMiddlePosition(cardIndex, count);
        }
    }
}
