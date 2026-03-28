using HermitMod.Cards;
using HermitMod.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// Deal {Damage} damage X times. If Fatal, get a Bounty. Exhaust.
/// Upgrade: 14 damage.
/// </summary>
public sealed class DeadOrAlive : HermitCard
{
    private const int DamageAmount = 10;
    private const int UpgradedDamageAmount = 14;

    public DeadOrAlive() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override bool HasEnergyCostX => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar((decimal)DamageAmount, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        int times = EnergyCost.CapturedXValue;

        for (int i = 0; i < times; i++)
        {
            await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target).Execute(ctx);

            // Stop if target died
            if (play.Target?.IsDead == true) break;
        }

        // If Fatal (target died), gain Bounty
        if (play.Target?.IsDead == true)
        {
            await PowerCmd.Apply<BountyPower>(Owner.Creature, 1, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UpgradedDamageAmount - DamageAmount);
    }
}
