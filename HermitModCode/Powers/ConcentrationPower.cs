using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Powers;

/// <summary>
/// The next Dead On card played this turn triggers its effect regardless of position.
/// Wears off at end of turn.
/// </summary>
public sealed class ConcentrationPower : HermitPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    /// <summary>
    /// Consumes one stack of Concentration. If no stacks remain, removes the power.
    /// Called from DeadOnOnPlayWrapperPatch when Dead On triggers via Concentration.
    /// </summary>
    public void ConsumeStack()
    {
        SetAmount(Amount - 1);
        if (Amount <= 0)
        {
            _ = PowerCmd.Remove(this);
        }
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side) return;
        await PowerCmd.Remove(this);
    }
}
