using HermitMod.Cards;
using HermitMod.Character;
using HermitMod.Patches;
using HermitMod.Utility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// Deal 5 damage. If last card triggered Dead On, draw a card.
/// Upgrade: 7 damage, gain Retain.
/// </summary>
public sealed class CalledShot : HermitCard
{
    protected override bool ShouldGlowGoldInternal => DeadOnCounter.GetIsLastCardDeadOn(Owner);
    
    private const int DamageAmount = 5;
    private const int UpgradedDamageAmount = 7;
    private const int DrawAmount = 1;

    public CalledShot() : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(DamageAmount, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromKeyword(HermitKeywords.DeadOn)]; 

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        HermitSfx.PlayGun2();
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target!).WithHermitGunHitFx().Execute(ctx);

        if (DeadOnCounter.GetIsLastCardDeadOn(Owner))
        {
            await CardPileCmd.Draw(ctx, DrawAmount, Owner, false);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UpgradedDamageAmount - DamageAmount);
        AddKeyword(CardKeyword.Retain);
    }
}
