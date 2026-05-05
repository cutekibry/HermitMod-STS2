using HermitMod.Cards;
using HermitMod.Character;
using HermitMod.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace HermitMod.Cards;

/// <summary>
/// Apply 2 Vulnerable to yourself. Gain 2 Rugged. Exhaust.
/// Upgrade: 1 Vulnerable, 1 Rugged.
/// </summary>
public sealed class Gestalt : HermitCard
{
    private const int BaseVulnAmount = 2;
    private const int UpgradedVulnAmount = 1;
    private const int RuggedAmount = 2;

    public Gestalt() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.None) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<RuggedPower>(RuggedAmount),
        new PowerVar<VulnerablePower>(BaseVulnAmount)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<VulnerablePower>(), HoverTipFactory.FromPower<RuggedPower>()];

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<RuggedPower>(ctx, Owner.Creature, DynamicVars["RuggedPower"].BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<VulnerablePower>(ctx, Owner.Creature, DynamicVars["VulnerablePower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["VulnerablePower"].UpgradeValueBy(UpgradedVulnAmount - BaseVulnAmount);
    }
}
