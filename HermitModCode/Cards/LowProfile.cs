using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// Gain 7 Block. Gain additional Block equal to 4 for each debuff on you.
/// Upgrade: 9 Block, 5 per debuff.
/// </summary>
public sealed class LowProfile : HermitCard
{
    private const int BlockAmount = 7;
    private const int UpgradedBlockAmount = 9;
    private const int ExtraBlock = 4;
    private const int UpgradedExtraBlock = 5;

    public LowProfile() : base(1, CardType.Skill, CardRarity.Common, TargetType.None) { }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(BlockAmount, ValueProp.Move),
        new DynamicVar("ExtraBlock", ExtraBlock),
        new CalculationBaseVar(BlockAmount),
        new CalculationExtraVar(ExtraBlock),
        new CalculatedBlockVar(ValueProp.Move).WithMultiplier(CountDebuffs)
    ];

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        await CreatureCmd.GainBlock(
            Owner.Creature,
            DynamicVars.CalculatedBlock.Calculate(Owner.Creature),
            DynamicVars.CalculatedBlock.Props,
            play
        );
    }

    private static decimal CountDebuffs(CardModel card, Creature? _)
    {
        return card.Owner.Creature.Powers.Count(p => p.Type == PowerType.Debuff || (p.Type == PowerType.Buff && p.Amount < 0));
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(UpgradedBlockAmount - BlockAmount);
        DynamicVars["ExtraBlock"].UpgradeValueBy(UpgradedExtraBlock - ExtraBlock);
        DynamicVars.CalculationBase.UpgradeValueBy(UpgradedBlockAmount - BlockAmount);
        DynamicVars.CalculationExtra.UpgradeValueBy(UpgradedExtraBlock - ExtraBlock);
    }
}
