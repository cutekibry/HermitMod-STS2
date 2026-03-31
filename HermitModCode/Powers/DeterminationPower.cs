using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace HermitMod.Powers;

/// <summary>
/// Whenever a debuff is applied to you, gain X Strength.
/// </summary>
public sealed class DeterminationPower : HermitPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPowerAmountChanged(
        PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        // Only respond to debuffs applied to our owner
        if (power.Owner != Owner) return;

        // Don't react to our own Strength grants (prevent infinite loop)
        if (power is StrengthPower) return;

        // Only debuffs
        if (power.Type != PowerType.Debuff) return;

        // Only when the debuff amount is positive (application, not reduction)
        if (amount <= 0) return;

        // Grant Strength equal to this power's amount
        await PowerCmd.Apply<StrengthPower>(Owner, Amount, Owner, null);
    }
}
