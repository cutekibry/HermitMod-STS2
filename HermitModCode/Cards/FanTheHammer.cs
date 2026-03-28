using HermitMod.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// X-cost. Deal 6 damage to a random enemy X+1 times.
/// Upgrade: 8 damage.
/// </summary>
public sealed class FanTheHammer : HermitCard
{
    private const int DamageAmount = 6;
    private const int UpgradedDamageAmount = 8;

    public FanTheHammer() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies) { }

    protected override bool HasEnergyCostX => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar((decimal)DamageAmount, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        int times = EnergyCost.CapturedXValue + 1;
        var rng = new Random();

        for (int i = 0; i < times; i++)
        {
            var enemies = CombatState.HittableEnemies.ToList();
            if (enemies.Count == 0) break;

            var target = enemies[rng.Next(enemies.Count)];

            await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Targeting(target)
                .Execute(ctx);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UpgradedDamageAmount - DamageAmount);
    }
}
