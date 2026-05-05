using System.Diagnostics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Relics;

/// <summary>
/// Whenever you apply a new debuff to an enemy, gain 3 Block.
/// </summary>
public sealed class RedScarf : HermitRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;

    private const int BlockAmount = 3;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(BlockAmount, ValueProp.Unpowered)];

    public override async Task BeforePowerAmountChanged(PowerModel power, decimal amount, Creature target, Creature? applier, CardModel? cardSource)
    {
        if(amount > 0 && target.IsEnemy && power.Type == PowerType.Debuff && (target.GetPower(power.Id)?.Amount ?? 0) == 0 && applier == Owner.Creature)
        {
            Flash();
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, null);
        }
    }
}
