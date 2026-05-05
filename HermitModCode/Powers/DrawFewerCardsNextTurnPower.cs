using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace HermitMod.Powers;

/// <summary>
/// Draw 1 fewer card next turn. 
/// </summary>
public sealed class DrawFewerCardsNextTurnPower : HermitPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        if (player != Owner.Player)
        {
            return count;
        }

        if (AmountOnTurnStart == 0)
        {
            return count;
        }

        return Math.Max(0m, count - Amount);
    }

    public override async Task AfterSideTurnStart(CombatSide side, ICombatState combatState)
    {
        if (side == Owner.Side && AmountOnTurnStart != 0)
            await PowerCmd.Remove(this);
    }
}
