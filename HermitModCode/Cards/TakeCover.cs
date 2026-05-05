using HermitMod.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// Add an upgraded X times Defend to your hand. It costs 0 in the combat.
/// Upgrade: Upgrade the generated Defend an additional time.
/// </summary>
public sealed class TakeCover : HermitCard
{
    public TakeCover() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.None) { }

    protected override bool HasEnergyCostX => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];


    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromCard<DefendHermit>(IsUpgraded),
        ..HoverTipFactory.FromEnchantment<Nimble>(),
    ];


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        var defend = CombatState!.CreateCard<DefendHermit>(Owner);
        defend.SetToFreeThisCombat();
        if(IsUpgraded)
            CardCmd.Upgrade(defend);
        CardCmd.Enchant<Nimble>(defend, 3 * EnergyCost.CapturedXValue);
        await CardPileCmd.AddGeneratedCardToCombat(defend, PileType.Hand, Owner);
    }
}
