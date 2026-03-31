using HermitMod.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace HermitMod.Cards;

/// <summary>
/// Apply 1 Weak and 1 Vulnerable to an enemy.
/// Upgrade: Apply 2 of each.
/// </summary>
public sealed class Glare : HermitCard
{
    public Glare() : base(0, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<WeakPower>(1m)];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<WeakPower>(), HoverTipFactory.FromPower<VulnerablePower>()];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<WeakPower>(play.Target, DynamicVars["WeakPower"].BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<VulnerablePower>(play.Target, DynamicVars["WeakPower"].BaseValue, Owner.Creature, this);
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        IsUpgraded ? [CardKeyword.Retain] : [];

    protected override void OnUpgrade()
    {
        // Upgrade only adds Retain (handled by CanonicalKeywords), no stat change
    }
}
