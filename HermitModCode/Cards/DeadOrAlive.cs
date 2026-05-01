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
/// Deal {Damage} damage X times. If Fatal, get a Bounty. Exhaust.
/// Upgrade: 14 damage.
/// </summary>
public sealed class DeadOrAlive : HermitCard
{
    private const int DamageAmount = 8;
    private const int UpgradedDamageAmount = 11;
    private const int BountyGold = 15;
    private const int UpgradedBountyGold = 25;

    public DeadOrAlive() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }

    protected override bool HasEnergyCostX => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<CardKeyword> CustomKeywords => [HermitKeywords.Bounty];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar((decimal)DamageAmount, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        int times = EnergyCost.CapturedXValue;

        for (int i = 0; i < times; i++)
        {
            await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target).WithHermitBluntLightHitFx().Execute(ctx);

            // Stop if target died
            if (play.Target?.IsDead == true) break;
        }

        // If Fatal (target died), gain gold and track Bounty
        if (play.Target?.IsDead == true)
        {
            int gold = IsUpgraded ? UpgradedBountyGold : BountyGold;
            await PlayerCmd.GainGold(gold, Owner);
            await PowerCmd.Apply<BountyPower>(ctx, Owner.Creature, gold, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UpgradedDamageAmount - DamageAmount);
    }
}
