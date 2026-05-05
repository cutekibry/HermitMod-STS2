using HermitMod.Cards;
using HermitMod.Character;
using HermitMod.Utility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// Deal 15 damage to ALL enemies. Stun any that don't intend to attack. Exhaust.
/// Upgrade: 20 damage.
/// </summary>
public sealed class RoundhouseKick : HermitCard
{
    private const int DamageAmount = 13;
    private const int UpgradedDamageAmount = 18;

    public RoundhouseKick() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(DamageAmount, ValueProp.Move)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [StunIntent.GetStaticHoverTip()];

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(CombatState!)
            .WithHermitBluntHeavyHitFx()
            .Execute(ctx);

        // Stun enemies that don't intend to attack
        foreach (var enemy in CombatState!.HittableEnemies)
        {
            if (enemy.IsDead) continue;
            var monster = enemy.Monster;
            if (monster == null) continue;

            // Check if the monster does NOT intend to attack
            if (!monster.IntendsToAttack)
            {
                await CreatureCmd.Stun(enemy);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UpgradedDamageAmount - DamageAmount);
    }
}
