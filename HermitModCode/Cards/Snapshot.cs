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
/// Deal 6 damage. Dead On: Gain Block equal to unblocked damage dealt.
/// Upgrade: 9 damage.
/// </summary>
public sealed class Snapshot : HermitCard
{
    public override bool HasDeadOn => true;

    private const int DamageAmount = 6;
    private const int UpgradedDamageAmount = 8;

    public Snapshot() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar((decimal)DamageAmount, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        HermitSfx.PlayGun1();

        // Record target HP before attack to calculate unblocked damage
        int hpBefore = play.Target?.CurrentHp ?? 0;

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(play.Target)
            .WithHermitGunHitFx()
            .Execute(ctx);

        if (DeadOnHelper.IsDeadOn && play.Target != null)
        {
            DeadOnHelper.IncrementDeadOnCount();
            // Calculate unblocked damage: how much HP the target actually lost
            int hpAfter = play.Target.IsDead ? 0 : play.Target.CurrentHp;
            int unblockedDamage = hpBefore - hpAfter;
            if (unblockedDamage > 0)
            {
                await CreatureCmd.GainBlock(Owner.Creature, (decimal)unblockedDamage, ValueProp.Move, play);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UpgradedDamageAmount - DamageAmount);
    }
}
