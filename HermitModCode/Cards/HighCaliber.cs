using HermitMod.Cards;
using HermitMod.Utility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// Deal 6 damage. Add a Strike+2 to your hand. Exhaust.
/// Upgrade: 9 damage and Strike+3.
/// </summary>
public sealed class HighCaliber : HermitCard
{
    private const int DamageAmount = 6;
    private const int UpgradedDamageAmount = 9;

    private const int SharpAmount = 6;

    public HighCaliber() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(DamageAmount, ValueProp.Move),
        new DynamicVar("SharpAmount", SharpAmount),
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromCard<StrikeHermit>(IsUpgraded),
        ..HoverTipFactory.FromEnchantment<Sharp>(),
    ];

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        HermitSfx.PlayGun1();
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target!).WithHermitGunHitFx().Execute(ctx);

        // Add a Strike to hand
        CardModel strike = CombatState!.CreateCard<StrikeHermit>(Owner);
        CardCmd.Enchant<Sharp>(strike, DynamicVars["SharpAmount"].BaseValue);
        if(IsUpgraded)
            CardCmd.Upgrade(strike);
        await CardPileCmd.AddGeneratedCardToCombat(strike, PileType.Hand, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UpgradedDamageAmount - DamageAmount);
    }
}
