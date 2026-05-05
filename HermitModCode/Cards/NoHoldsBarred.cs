using HermitMod.Character;
using HermitMod.Utility;
using HermitMod.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.HoverTips;

namespace HermitMod.Cards;

/// <summary>
/// Deal 19 damage to ALL enemies. Apply 5 Bruise to ALL enemies.
/// Upgrade: 23 damage. 6 Bruise.
/// </summary>
public sealed class NoHoldsBarred : HermitCard
{
    private const int DamageAmount = 19;
    private const int UpgradedDamageAmount = 23;
    private const int BruiseAmount = 5;
    private const int UpgradedBruiseAmount = 6;
    private const int EnergyLossNextTurn = 1;

    public NoHoldsBarred() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(DamageAmount, ValueProp.Move), new PowerVar<BruisePower>(BruiseAmount), new EnergyVar(EnergyLossNextTurn)];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<BruisePower>()];

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(CombatState!)
            .WithHermitSlashHitFx()
            .Execute(ctx);

        foreach (var enemy in CombatState!.HittableEnemies)
        {
            await PowerCmd.Apply<BruisePower>(ctx, enemy, BruiseAmount, Owner.Creature, this);
        }
        await PowerCmd.Apply<DrainedPower>(ctx, Owner.Creature, DynamicVars.Energy.BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UpgradedDamageAmount - DamageAmount);
        DynamicVars["BruisePower"].UpgradeValueBy(UpgradedBruiseAmount - BruiseAmount);
    }
}
