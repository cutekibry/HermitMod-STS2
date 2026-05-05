using HermitMod.Cards;
using HermitMod.Patches;
using HermitMod.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// Gain 8 Block. Dead On: Gain 2 additional Plating.
/// Upgrade: 10 Block, 3 additional Plating.
/// </summary>
public sealed class Dive : HermitCard
{
    public override bool HasDeadOn => true;

    private const int BlockAmount = 8;
    private const int UpgradedBlockAmount = 10;
    private const int PlatingAmount = 2;
    private const int UpgradedPlatingAmount = 3;

    public Dive() : base(1, CardType.Skill, CardRarity.Common, TargetType.None) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(BlockAmount, ValueProp.Move),
        new PowerVar<PlatingPower>(PlatingAmount),
    ];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromPower<PlatingPower>(),
    ];

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
    }
    protected override async Task AfterPlayInternalIfDeadOn(PlayerChoiceContext ctx, CardPlay play)
    {
        await PowerCmd.Apply<PlatingPower>(ctx, Owner.Creature, DynamicVars["PlatingPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(UpgradedBlockAmount - BlockAmount);
        DynamicVars["PlatingPower"].UpgradeValueBy(UpgradedPlatingAmount - PlatingAmount);
    }
}
