using HermitMod.Cards;
using HermitMod.Character;
using HermitMod.Utility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Cards;

/// <summary>
/// Add ALL Strikes and Defends from your draw pile to your hand. Exhaust.
/// Upgrade: Cost reduced from 2 to 1.
/// </summary>
public sealed class FullyLoaded : HermitCard
{
    public FullyLoaded() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.None) { }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromKeyword(HermitKeywords.Strike), HoverTipFactory.FromKeyword(HermitKeywords.Defend)];

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        HermitSfx.PlaySpin();
        HermitSfx.PlayReload();
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        var drawPile = PileType.Draw.GetPile(Owner);
        if (drawPile == null) return;

        // Find all Strikes and Defends in draw pile
        var strikesAndDefends = drawPile.Cards
            .Where(c => c.Tags.Contains(CardTag.Strike) || c.Tags.Contains(CardTag.Defend))
            .ToList();

        foreach (var card in strikesAndDefends)
        {
            await CardPileCmd.Add(card, PileType.Hand);
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
