using HermitMod.Cards;
using HermitMod.Patches;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// Gain 8 Block. Dead On: Gain 4 additional Block.
/// Upgrade: 10 Block, 6 additional Block.
/// </summary>
public sealed class Dive : HermitCard
{
    public override bool HasDeadOn => true;

    private const int BlockAmount = 8;
    private const int UpgradedBlockAmount = 10;
    private const int DeadOnBlock = 4;
    private const int UpgradedDeadOnBlock = 6;

    public Dive() : base(1, CardType.Skill, CardRarity.Common, TargetType.None) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar((decimal)BlockAmount, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);

        if (DeadOnHelper.IsDeadOn)
        {
            DeadOnHelper.IncrementDeadOnCount();
            int bonus = IsUpgraded ? UpgradedDeadOnBlock : DeadOnBlock;
            await CreatureCmd.GainBlock(Owner.Creature, bonus, ValueProp.Unpowered, null, false);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(UpgradedBlockAmount - BlockAmount);
    }
}
