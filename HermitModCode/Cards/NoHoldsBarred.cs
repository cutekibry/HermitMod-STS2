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
/// Deal 12 damage to ALL enemies. Apply 2 Bruise to ALL enemies.
/// Upgrade: 16 damage.
/// </summary>
public sealed class NoHoldsBarred : HermitCard
{
    private const int DamageAmount = 12;
    private const int UpgradedDamageAmount = 16;
    private const int BruiseAmount = 2;

    public NoHoldsBarred() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar((decimal)DamageAmount, ValueProp.Move), new PowerVar<BruisePower>((decimal)BruiseAmount)];

    protected override IEnumerable<CardKeyword> CustomKeywords => [HermitKeywords.Bruise];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(CombatState)
            .WithHermitSlashHitFx()
            .Execute(ctx);

        foreach (var enemy in CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<BruisePower>(ctx, enemy, BruiseAmount, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UpgradedDamageAmount - DamageAmount);
    }
}
