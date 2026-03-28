using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Powers;

/// <summary>
/// Rugged: Reduces next instance(s) of attack damage taken to 2.
/// Each stack = one instance of damage reduction.
/// Uses ModifyHpLostBeforeOsty to cap incoming damage and consume stacks.
/// </summary>
public sealed class RuggedPower : HermitPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    /// <summary>
    /// When the owner would lose HP from an attack, cap the damage to 2 and consume a stack.
    /// </summary>
    public override decimal ModifyHpLostBeforeOsty(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        // Only protect the owner, only when we have stacks, only for actual damage
        if (target != Owner || Amount <= 0 || amount <= 0m)
            return amount;

        // Consume a stack
        Amount--;

        // If no stacks remain, mark for removal (will be cleaned up)
        if (Amount <= 0)
        {
            // Schedule removal — can't await in a non-async method, so use fire-and-forget
            _ = PowerCmd.Remove(this);
        }

        // Cap damage to 2
        return Math.Min(amount, 2m);
    }
}
