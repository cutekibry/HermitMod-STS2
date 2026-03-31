using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using HermitMod.Cards;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Patches;

/// <summary>
/// Reduces ShortFuse cost by 1 when a Basic-rarity Attack or Skill is played.
/// </summary>
[HarmonyPatch(typeof(Hook), "AfterCardPlayed")]
public static class ShortFuseCostReductionPatch
{
    [HarmonyPostfix]
    public static void Postfix(CardPlay cardPlay)
    {
        if (cardPlay?.Card?.Owner == null)
            return;

        CardModel card = cardPlay.Card;

        // Only trigger on Basic rarity cards that are Attacks or Skills
        if ((int)card.Rarity != 1 || ((int)card.Type != 1 && (int)card.Type != 2))
            return;

        Player owner = card.Owner;
        IReadOnlyList<CardModel> handCards = PileTypeExtensions.GetPile(PileType.Hand, owner).Cards;
        if (handCards == null)
            return;

        foreach (ShortFuse shortFuse in handCards.OfType<ShortFuse>())
        {
            ((CardModel)shortFuse).EnergyCost.AddThisTurnOrUntilPlayed(-1, true);
        }
    }
}
