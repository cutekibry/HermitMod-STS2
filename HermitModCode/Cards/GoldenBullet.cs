using HermitMod.Utility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// Deal 20 damage. If Fatal, permanently reduce this card's cost by 1. Exhaust.
/// Upgrade: 28 damage.
/// </summary>
public sealed class GoldenBullet : HermitCard
{
    private const int DamageAmount = 18;
    private const int UpgradedDamageAmount = 24;

    public GoldenBullet() : base(3, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar((decimal)DamageAmount, ValueProp.Move)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        HermitSfx.PlayGun1();
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target).WithHermitGunHitFx().Execute(ctx);

        // Fatal: permanently reduce cost by 1 (synced to deck version so it persists across combats)
        if (play.Target?.IsDead == true)
        {
            ApplyCostReduction();
            // Sync to the deck version so the reduction persists after combat
            (DeckVersion as GoldenBullet)?.ApplyCostReduction();
        }
    }

    public void ApplyCostReduction()
    {
        EnergyCost.UpgradeBy(-1);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UpgradedDamageAmount - DamageAmount);
    }
}
