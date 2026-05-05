using HermitMod.Patches;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// Gain 12 Block. Dead On: Gain 5 Gold. Exhaust.
/// Upgrade: 15 Block, 10 Gold.
/// (Original STS1: Plated Armor 4/5. Adapted to Block since Plated Armor doesn't exist in STS2.)
/// </summary>
public sealed class Scavenge : HermitCard
{
    public override bool HasDeadOn => true;

    private const int PlatingAmount = 6;
    private const int UpgradedPlatingAmount = 8;
    private const int GoldAmount = 5;
    private const int UpgradedGoldAmount = 10;

    public Scavenge() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.None) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<PlatingPower>(PlatingAmount),
        new GoldVar(GoldAmount),
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<PlatingPower>()];

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<PlatingPower>(ctx, Owner.Creature, DynamicVars["PlatingPower"].BaseValue, Owner.Creature, this);
    }

    protected override async Task AfterPlayInternalIfDeadOn(PlayerChoiceContext ctx, CardPlay play)
    {
        await PlayerCmd.GainGold(DynamicVars.Gold.BaseValue, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["PlatingPower"].UpgradeValueBy(UpgradedPlatingAmount - PlatingAmount);
        DynamicVars.Gold.UpgradeValueBy(UpgradedGoldAmount - GoldAmount);
    }
}
