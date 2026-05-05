using BaseLib.Abstracts;
using HermitMod.Patches;
using HermitMod.Utility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// Deal 6 damage. Dead On: Gain Block equal to unblocked damage dealt first.
/// Upgrade: 8 damage.
/// </summary>
public sealed class Snapshot : HermitCard, ITranscendenceCard
{
    public override bool HasDeadOn => true;

    private const int DamageAmount = 6;
    private const int UpgradedDamageAmount = 8;

    public Snapshot() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(DamageAmount, ValueProp.Move)];

    protected override async Task BeforePlayInternalIfDeadOn(PlayerChoiceContext ctx, CardPlay play)
    {
        IRunState runState = IRunState.GetFrom([play.Target!, Owner.Creature]);
        decimal modifiedDamage = Hook.ModifyDamage(runState, CombatState, play.Target!, Owner.Creature, DynamicVars.Damage.BaseValue, ValueProp.Move, this, ModifyDamageHookType.All, CardPreviewMode.None, out _);
        int unblockedDamage = (int)(modifiedDamage - play.Target!.Block);
        await CreatureCmd.GainBlock(Owner.Creature, unblockedDamage, ValueProp.Move, play);
    }
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        HermitSfx.PlayGun1();
        
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(play.Target!)
            .WithHermitGunHitFx()
            .Execute(ctx);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UpgradedDamageAmount - DamageAmount);
    }

    public CardModel GetTranscendenceTransformedCard()
    {
        return ModelDb.Card<OneFlash>();
    }
}
