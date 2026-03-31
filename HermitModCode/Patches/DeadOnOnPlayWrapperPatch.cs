using System.Collections.Generic;
using HarmonyLib;
using HermitMod.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Patches;

/// <summary>
/// Prefix patch on CardModel.OnPlayWrapper to capture the card's hand position
/// BEFORE AddDuringManualCardPlay removes it from the hand.
/// Also checks for ConcentrationPower -- if present, Dead On auto-triggers.
/// </summary>
[HarmonyPatch(typeof(CardModel), "OnPlayWrapper")]
public static class DeadOnOnPlayWrapperPatch
{
    [HarmonyPrefix]
    public static void Prefix(CardModel __instance, Creature? target, bool isAutoPlay)
    {
        DeadOnHelper.SetDeadOn(value: false);

        if (__instance?.Owner == null)
            return;

        if (DeadOnHelper.ForceNextDeadOn)
        {
            DeadOnHelper.ForceNextDeadOn = false;
            DeadOnHelper.SetDeadOn(value: true);
            return;
        }

        Player owner = __instance.Owner;
        Creature creature = owner.Creature;

        if (creature?.Powers != null)
        {
            foreach (PowerModel power in creature.Powers)
            {
                if (power is ConcentrationPower concentrationPower && concentrationPower.Amount > 0)
                {
                    DeadOnHelper.SetDeadOn(value: true);
                    MainFile.Logger.Info($"Dead On triggered via Concentration (amount: {concentrationPower.Amount})");
                    concentrationPower.ConsumeStack();
                    return;
                }
            }
        }

        CardPile pile = __instance.Pile;
        if (pile == null || pile.Type != PileType.Hand)
            return;

        IReadOnlyList<CardModel> cards = pile.Cards;
        if (cards == null)
            return;

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
            bool deadOn = DeadOnHelper.IsMiddlePosition(cardIndex, count);
            DeadOnHelper.SetDeadOn(deadOn);
        }
    }
}
