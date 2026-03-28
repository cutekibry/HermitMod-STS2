using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Powers;

/// <summary>
/// First 4 playable cards drawn at the start of each turn cost X less that turn.
/// X = power stack amount.
/// </summary>
public sealed class EternalPower : HermitPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private int _cardsReducedThisTurn;

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        _cardsReducedThisTurn = 0;
        return Task.CompletedTask;
    }

    public override Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (_cardsReducedThisTurn >= 4) return Task.CompletedTask;
        if (card.EnergyCost.CostsX) return Task.CompletedTask;

        // Reduce cost by Amount (power stacks) this turn
        card.EnergyCost.AddThisTurnOrUntilPlayed(-Amount, reduceOnly: true);
        _cardsReducedThisTurn++;

        return Task.CompletedTask;
    }
}
