using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Relics;

/// <summary>
/// At the end of your turn, gain 2 Block.
/// </summary>
public sealed class BrassTacks : HermitRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;

    private const int BlockAmount = 2;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(BlockAmount, ValueProp.Unpowered)];

    public override async Task BeforeTurnEndEarly(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Creature.Side || Owner?.Creature == null) return;

        Flash();
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, null, false);
    }
}
