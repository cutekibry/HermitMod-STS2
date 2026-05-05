using HermitMod.Character;
using HermitMod.Patches;
using HermitMod.Utility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// Deal 7 damage. Repeat on a random enemy for each Dead On effect this turn.
/// Upgrade: 9 damage.
/// </summary>
public sealed class Ricochet : HermitCard
{
    private const int DamageAmount = 7;
    private const int UpgradedDamageAmount = 9;

    public Ricochet() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(DamageAmount, ValueProp.Move),
        new CalculationBaseVar(0),
        new CalculationExtraVar(1),
        new CalculatedVar("CalculatedHits").WithMultiplier(CountDeadOnEffects)
    ];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromKeyword(HermitKeywords.DeadOn)];

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);

        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);

        int extraHitCount = (int)((CalculatedVar)DynamicVars["CalculatedHits"]).Calculate(play.Target);

        HermitSfx.PlayGun2();
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(play.Target)
            .WithHermitGunHitFx()
            .Execute(ctx);

        for (int i = 0; i < extraHitCount; i++)
        {
            var enemies = CombatState?.HittableEnemies.ToList();
            if (enemies == null || enemies.Count == 0) break;

            HermitSfx.PlayGun3();
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .TargetingRandomOpponents(CombatState!)
                .WithHermitGunHitFx()
                .Execute(ctx);
        }
    }

    private static decimal CountDeadOnEffects(CardModel card, Creature? _)
    {
        return DeadOnCounter.GetCounter(card.Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UpgradedDamageAmount - DamageAmount);
    }
}
