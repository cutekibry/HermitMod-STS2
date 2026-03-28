using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Powers;

/// <summary>
/// Sniper Focus: Your single-target attacks deal 3 additional damage per stack.
/// </summary>
public sealed class SniperFocusPower : HermitPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        // Only boost single-target attacks from the owner
        if (dealer != Owner || cardSource == null)
            return amount;

        // Only boost single-target cards
        if (cardSource.TargetType != TargetType.AnyEnemy)
            return amount;

        // Add 3 damage per stack
        return amount + (3m * Amount);
    }
}
