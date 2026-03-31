using HermitMod.Cards;
using HermitMod.Patches;
using HermitMod.Utility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// Deal 5 damage. If last card triggered Dead On, draw a card.
/// Upgrade: 7 damage, gain Retain.
/// </summary>
public sealed class CalledShot : HermitCard
{
    public override bool HasDeadOn => true;

    private const int DamageAmount = 5;
    private const int UpgradedDamageAmount = 7;
    private const int DrawAmount = 1;

    public CalledShot() : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar((decimal)DamageAmount, ValueProp.Move)];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        IsUpgraded ? [CardKeyword.Retain] : [];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        HermitSfx.PlayGun2();
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target).WithHermitGunHitFx().Execute(ctx);

        // If the previous card triggered Dead On (counter > 0 means at least one Dead On happened)
        if (DeadOnHelper.DeadOnTriggersThisTurn > 0)
        {
            await CardPileCmd.Draw(ctx, DrawAmount, Owner, false);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UpgradedDamageAmount - DamageAmount);
        // Gain Retain keyword on upgrade
    }
}
