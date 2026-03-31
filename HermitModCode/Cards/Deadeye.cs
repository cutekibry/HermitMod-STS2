using HermitMod.Cards;
using HermitMod.Patches;
using HermitMod.Utility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// Deal 7 damage. Dead On: Gain 1 Strength.
/// Upgrade: 10 damage, gain 2 Strength.
/// </summary>
public sealed class Deadeye : HermitCard
{
    public override bool HasDeadOn => true;

    private const int DamageAmount = 5;
    private const int UpgradedDamageAmount = 6;
    private const int StrengthAmt = 2;
    private const int UpgradedStrengthAmt = 3;

    public Deadeye() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar((decimal)DamageAmount, ValueProp.Move), new PowerVar<StrengthPower>((decimal)StrengthAmt)];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];

    private int CurrentStrength => IsUpgraded ? UpgradedStrengthAmt : StrengthAmt;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(play.Target)
            .WithHermitBluntHeavyHitFx()
            .Execute(ctx);

        if (DeadOnHelper.IsDeadOn)
        {
            DeadOnHelper.IncrementDeadOnCount();
            await PowerCmd.Apply<StrengthPower>(Owner.Creature, CurrentStrength, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UpgradedDamageAmount - DamageAmount);
    }
}
