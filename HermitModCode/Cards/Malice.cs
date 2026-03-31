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
/// Deal 9 damage to ALL enemies. Apply 2 Bruise to ALL enemies.
/// Upgrade: 12 damage, 3 Bruise.
/// </summary>
public sealed class Malice : HermitCard
{
    private const int DamageAmount = 9;
    private const int UpgradedDamageAmount = 12;
    private const int BruiseAmount = 2;

    public Malice() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar((decimal)DamageAmount, ValueProp.Move)];

    protected override IEnumerable<CardKeyword> CustomKeywords => [HermitKeywords.Bruise];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(CombatState)
            .WithHermitFireHitFx()
            .Execute(ctx);

        foreach (var enemy in CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<BruisePower>(enemy, BruiseAmount, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UpgradedDamageAmount - DamageAmount);
    }
}
