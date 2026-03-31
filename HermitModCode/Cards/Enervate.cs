using HermitMod.Cards;
using HermitMod.Patches;
using HermitMod.Utility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// Deal 8 damage. Dead On: Gain 1 Energy and draw a card.
/// Upgrade: 12 damage.
/// </summary>
public sealed class Enervate : HermitCard
{
    public override bool HasDeadOn => true;

    private const int DamageAmount = 7;
    private const int UpgradedDamageAmount = 10;

    public Enervate() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar((decimal)DamageAmount, ValueProp.Move),
    ];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target).WithHermitFireHitFx().Execute(ctx);

        if (DeadOnHelper.IsDeadOn)
        {
            DeadOnHelper.IncrementDeadOnCount();
            await PlayerCmd.GainEnergy(1, Owner);
            await CardPileCmd.Draw(ctx, 1, Owner, false);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UpgradedDamageAmount - DamageAmount);
    }
}
