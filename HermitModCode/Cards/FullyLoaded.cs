using HermitMod.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// Deal 12 damage. Draw 2 cards.
/// Upgrade: 16 damage, Draw 3.
/// </summary>
public sealed class FullyLoaded : HermitCard
{
    private const int DamageAmount = 12;
    private const int UpgradedDamageAmount = 16;
    private const int DrawCount = 2;
    private const int UpgradedDrawCount = 3;

    public FullyLoaded() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar((decimal)DamageAmount, ValueProp.Move),
        new CardsVar(DrawCount)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target).Execute(ctx);
        await CardPileCmd.Draw(ctx, DynamicVars.Cards.IntValue, Owner, false);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UpgradedDamageAmount - DamageAmount);
        DynamicVars.Cards.UpgradeValueBy(UpgradedDrawCount - DrawCount);
    }
}
