using HermitMod.Cards;
using HermitMod.Utility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// Deal 7 damage. Draw 1 card.
/// Upgrade: 10 damage.
/// </summary>
public sealed class Quickdraw : HermitCard
{
    private const int DamageAmount = 9;
    private const int UpgradedDamageAmount = 11;

    public Quickdraw() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    private const int DrawCount = 2;
    private const int UpgradedDrawCount = 3;

    private int CurrentDraw => IsUpgraded ? UpgradedDrawCount : DrawCount;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar((decimal)DamageAmount, ValueProp.Move), new CardsVar(DrawCount)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target).WithHermitBluntLightHitFx().Execute(ctx);
        await CardPileCmd.Draw(ctx, CurrentDraw, Owner, false);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UpgradedDamageAmount - DamageAmount);
        DynamicVars.Cards.UpgradeValueBy(UpgradedDrawCount - DrawCount);
    }
}
