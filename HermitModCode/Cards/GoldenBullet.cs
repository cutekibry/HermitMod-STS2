using HermitMod.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace HermitMod.Cards;

/// <summary>
/// Deal 20 damage. If Fatal, permanently reduce this card's cost by 1. Exhaust.
/// Upgrade: 28 damage.
/// </summary>
public sealed class GoldenBullet : HermitCard
{
    private const int DamageAmount = 20;
    private const int UpgradedDamageAmount = 28;

    public GoldenBullet() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar((decimal)DamageAmount, ValueProp.Move)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target).Execute(ctx);

        // Fatal: permanently reduce base cost by 1
        if (play.Target?.IsDead == true)
        {
            int currentBase = EnergyCost.GetWithModifiers(CostModifiers.None);
            int newCost = Math.Max(0, currentBase - 1);
            EnergyCost.SetCustomBaseCost(newCost);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UpgradedDamageAmount - DamageAmount);
    }
}
