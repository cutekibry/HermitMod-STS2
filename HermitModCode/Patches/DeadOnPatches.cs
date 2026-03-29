using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using HermitMod.Powers;

namespace HermitMod.Patches;

/// <summary>
/// Tracks Dead On state for the current card play.
/// Dead On triggers when a card is played from the middle position of the hand.
/// For odd hand sizes: the exact middle position.
/// For even hand sizes: either of the two center positions.
///
/// Patches CardModel.OnPlayWrapper (prefix) to capture the card's hand position
/// BEFORE the card is moved from hand to play pile. The old Hook.BeforeCardPlayed
/// patch was too late — by that point AddDuringManualCardPlay had already removed
/// the card from the hand.
///
/// Also patches Hook.AfterCardPlayed to reset the flag, and
/// Hook.BeforeSideTurnStart to reset the per-turn trigger counter.
/// </summary>
public static class DeadOnHelper
{
    private static bool _currentCardIsDeadOn;
    private static int _deadOnTriggersThisTurn;

    /// <summary>
    /// When true, the next card played will have Dead On forced on.
    /// Used by Cheat card to propagate Dead On to the selected card.
    /// Reset after the next card's Dead On check.
    /// </summary>
    public static bool ForceNextDeadOn { get; set; }

    public static bool IsDeadOn => _currentCardIsDeadOn;
    public static int DeadOnTriggersThisTurn => _deadOnTriggersThisTurn;

    public static void IncrementDeadOnCount()
    {
        _deadOnTriggersThisTurn++;
    }

    public static void ResetTurnCount()
    {
        _deadOnTriggersThisTurn = 0;
    }

    /// <summary>
    /// Determines if a card at the given index is in the middle of a hand of the given size.
    /// For odd hand sizes: exact middle position.
    /// For even hand sizes: either of the two middle positions.
    /// </summary>
    public static bool IsMiddlePosition(int cardIndex, int handSize)
    {
        if (handSize <= 0) return false;
        if (handSize == 1) return true;

        int middle = handSize / 2;
        if (handSize % 2 == 1)
        {
            return cardIndex == middle;
        }
        else
        {
            return cardIndex == middle - 1 || cardIndex == middle;
        }
    }

    internal static void SetDeadOn(bool value)
    {
        _currentCardIsDeadOn = value;
    }
}

/// <summary>
/// Prefix patch on CardModel.OnPlayWrapper to capture the card's hand position
/// BEFORE AddDuringManualCardPlay removes it from the hand.
/// This is the earliest point where we know the card is being played and it's
/// still in the hand pile at its correct position.
/// Also checks for ConcentrationPower — if present, Dead On auto-triggers.
/// </summary>
[HarmonyPatch(typeof(CardModel), nameof(CardModel.OnPlayWrapper))]
public static class DeadOnOnPlayWrapperPatch
{
    [HarmonyPrefix]
    public static void Prefix(CardModel __instance, Creature? target, bool isAutoPlay)
    {
        // Default to false at the start of every card play
        DeadOnHelper.SetDeadOn(false);

        if (__instance?.Owner == null) return;

        // Check if ForceNextDeadOn was set (by Cheat card)
        if (DeadOnHelper.ForceNextDeadOn)
        {
            DeadOnHelper.ForceNextDeadOn = false;
            DeadOnHelper.SetDeadOn(true);
            return;
        }

        var owner = __instance.Owner;

        // Check if the owner has ConcentrationPower — if so, Dead On always triggers
        var concentrationPower = owner.Creature?.Powers?
            .OfType<ConcentrationPower>()
            .FirstOrDefault();

        if (concentrationPower != null && concentrationPower.Amount > 0)
        {
            DeadOnHelper.SetDeadOn(true);
            return;
        }

        // Card must be in the hand to check position
        var pile = __instance.Pile;
        if (pile == null || pile.Type != PileType.Hand) return;

        var handCards = pile.Cards;
        if (handCards == null) return;

        int handSize = handCards.Count;
        int cardIndex = -1;

        // Find the card's index in the hand
        for (int i = 0; i < handCards.Count; i++)
        {
            if (handCards[i] == __instance)
            {
                cardIndex = i;
                break;
            }
        }

        if (cardIndex < 0) return;

        bool isMiddle = DeadOnHelper.IsMiddlePosition(cardIndex, handSize);
        DeadOnHelper.SetDeadOn(isMiddle);
    }
}

/// <summary>
/// Postfix patch on Hook.AfterCardPlayed to reset the Dead On flag
/// after the card play has fully resolved.
/// </summary>
[HarmonyPatch(typeof(Hook), nameof(Hook.AfterCardPlayed))]
public static class DeadOnAfterCardPlayedPatch
{
    [HarmonyPostfix]
    public static void Postfix(CombatState combatState, PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        DeadOnHelper.SetDeadOn(false);
    }
}

/// <summary>
/// Patch on Hook.BeforeSideTurnStart to reset the Dead On trigger counter
/// at the start of each turn.
/// </summary>
[HarmonyPatch(typeof(Hook), nameof(Hook.BeforeSideTurnStart))]
public static class DeadOnTurnResetPatch
{
    [HarmonyPrefix]
    public static void Prefix()
    {
        DeadOnHelper.ResetTurnCount();
    }
}
