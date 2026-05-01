using HermitMod.Cards;
using HermitMod.Character;
using HermitMod.Utility;
using HermitMod.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// Concentrate. Deal 5 damage twice.
/// Upgrade: 7 damage.
/// </summary>
public sealed class TrackingShot : HermitCard
{
    private const int DamageAmount = 5;
    private const int UpgradedDamageAmount = 7;
    private const int HitCount = 2;

    public TrackingShot() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar((decimal)DamageAmount, ValueProp.Move)];

    protected override IEnumerable<CardKeyword> CustomKeywords => [HermitKeywords.Concentrate];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        // Concentrate: apply Concentration power
        await PowerCmd.Apply<ConcentrationPower>(ctx, Owner.Creature, 1, Owner.Creature, this);

        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);

        for (int i = 0; i < HitCount; i++)
        {
            if (i == 0)
                HermitSfx.PlayGun3();
            else
                HermitSfx.PlayGun1();
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target).WithHermitGunHitFx().Execute(ctx);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UpgradedDamageAmount - DamageAmount);
    }
}
