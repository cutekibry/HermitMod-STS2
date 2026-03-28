using System.Linq;
using HarmonyLib;
using HermitMod.Cards;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Patches;

/// <summary>
/// After any card is played, check if it was a Strike or Defend.
/// If so, reduce all ShortFuse cards in hand by 1 for this turn.
/// </summary>
[HarmonyPatch(typeof(Hook), nameof(Hook.AfterCardPlayed))]
public static class ShortFuseCostReductionPatch
{
    [HarmonyPostfix]
    public static void Postfix(CardPlay cardPlay)
    {
        if (cardPlay?.Card?.Owner == null) return;

        var card = cardPlay.Card;

        // Check if the played card is a Strike or Defend (basic cards)
        bool isStrikeOrDefend = card.Rarity == CardRarity.Basic &&
            (card.Type == CardType.Attack || card.Type == CardType.Skill);

        if (!isStrikeOrDefend) return;

        var owner = card.Owner;
        var handCards = PileType.Hand.GetPile(owner).Cards;
        if (handCards == null) return;

        // Reduce cost of all ShortFuse cards in hand by 1 this turn
        foreach (var handCard in handCards.OfType<ShortFuse>())
        {
            handCard.EnergyCost.AddThisTurnOrUntilPlayed(-1, reduceOnly: true);
        }
    }
}
