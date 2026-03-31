using HermitMod.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// Gain 12 Block. Add a Decay to your hand. Exhaust.
/// Upgrade: 16 Block.
/// </summary>
public sealed class Manifest : HermitCard
{
    private const int BlockAmount = 16;
    private const int UpgradedBlockAmount = 20;

    public Manifest() : base(2, CardType.Skill, CardRarity.Common, TargetType.None) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar((decimal)BlockAmount, ValueProp.Move)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromCard<MegaCrit.Sts2.Core.Models.Cards.Decay>()];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);

        // Add a Decay (base game curse) to the player's hand
        var decay = CombatState.CreateCard<MegaCrit.Sts2.Core.Models.Cards.Decay>(Owner);
        await CardPileCmd.AddGeneratedCardToCombat(decay, PileType.Hand, addedByPlayer: true);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(UpgradedBlockAmount - BlockAmount);
    }
}
