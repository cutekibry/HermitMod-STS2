using HarmonyLib;
using HermitMod.Cards;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Patches;

/// <summary>
/// Makes cards with Dead On effects glow gold when they are in the Dead On position
/// (middle of the hand). Patches CardModel.ShouldGlowGold getter.
/// </summary>
[HarmonyPatch(typeof(CardModel), nameof(CardModel.ShouldGlowGold), MethodType.Getter)]
public static class DeadOnGlowPatch
{
    [HarmonyPostfix]
    public static void Postfix(CardModel __instance, ref bool __result)
    {
        // If already glowing gold for another reason, don't interfere
        if (__result) return;

        // Only glow Hermit cards (they have Dead On mechanics)
        if (__instance is not HermitCard) return;

        // Check if the card is in the hand
        var pile = __instance.Pile;
        if (pile == null || pile.Type != PileType.Hand) return;

        var handCards = pile.Cards;
        if (handCards == null || handCards.Count == 0) return;

        int handSize = handCards.Count;
        int cardIndex = -1;

        for (int i = 0; i < handCards.Count; i++)
        {
            if (handCards[i] == __instance)
            {
                cardIndex = i;
                break;
            }
        }

        if (cardIndex < 0) return;

        // Glow gold if the card is in the Dead On position
        __result = DeadOnHelper.IsMiddlePosition(cardIndex, handSize);
    }
}
