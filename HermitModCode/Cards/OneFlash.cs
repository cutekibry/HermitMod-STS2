using HermitMod.Patches;
using HermitMod.Utility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// Deal 12 damage. Gain Block equal to unblocked damage dealt first.
/// Dead On: Gain 4 Strength.
/// Upgrade: 16 damage, gain 5 Strength.
/// </summary>
public sealed class OneFlash : HermitCard
{
    public override bool HasDeadOn => true;

    private const int DamageAmount = 12;
    private const int UpgradedDamageAmount = 16;
    private const int StrengthAmount = 4;
    private const int UpgradedStrengthAmount = 5;

    public OneFlash() : base(1, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(DamageAmount, ValueProp.Move),
        new PowerVar<StrengthPower>(StrengthAmount)
    ];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        HermitSfx.PlayGun1();

        IRunState runState = IRunState.GetFrom([play.Target!, Owner.Creature]);
        decimal modifiedDamage = Hook.ModifyDamage(runState, CombatState, play.Target!, Owner.Creature, DynamicVars.Damage.BaseValue, ValueProp.Move, this, ModifyDamageHookType.All, CardPreviewMode.None, out _);
        int unblockedDamage = Math.Max(0, (int)(modifiedDamage - play.Target!.Block));
        await CreatureCmd.GainBlock(Owner.Creature, unblockedDamage, ValueProp.Move, play);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(play.Target!)
            .WithHermitGunHitFx()
            .Execute(ctx);
    }
    protected override async Task AfterPlayInternalIfDeadOn(PlayerChoiceContext ctx, CardPlay play)
    {
        await PowerCmd.Apply<StrengthPower>(ctx, Owner.Creature, DynamicVars["StrengthPower"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UpgradedDamageAmount - DamageAmount);
        DynamicVars["StrengthPower"].UpgradeValueBy(UpgradedStrengthAmount - StrengthAmount);
    }
}
