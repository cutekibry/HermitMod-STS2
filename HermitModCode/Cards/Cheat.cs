using HermitMod.Patches;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Cards;

/// <summary>
/// Look at the top 3 cards in your draw pile. Choose one to play.
/// Dead On: Also trigger its Dead On effect.
/// Upgrade: Top 5 cards.
/// </summary>
public sealed class Cheat : HermitCard
{
    public override bool HasDeadOn => true;

    private const int CardCount = 3;
    private const int UpgradedCardCount = 5;

    public Cheat() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.None) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(CardCount)];

    private int CurrentCardCount => IsUpgraded ? UpgradedCardCount : CardCount;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        var drawPile = PileType.Draw.GetPile(Owner);
        if (drawPile == null || drawPile.Cards.Count == 0)
            return;

        // Draw the top N cards to hand so they become visible for selection
        int toDraw = Math.Min(CurrentCardCount, drawPile.Cards.Count);
        int handBefore = PileType.Hand.GetPile(Owner)?.Cards.Count ?? 0;

        await CardPileCmd.Draw(ctx, toDraw, Owner, false);

        var hand = PileType.Hand.GetPile(Owner);
        if (hand == null) return;

        // The newly drawn cards are at the end of the hand
        int handAfter = hand.Cards.Count;
        int actualDrawn = handAfter - handBefore;
        if (actualDrawn <= 0) return;

        var drawnCards = hand.Cards.Skip(handBefore).Take(actualDrawn).ToList();

        if (drawnCards.Count == 1)
        {
            // Only one card drawn — auto-select it
            if (DeadOnHelper.IsDeadOn)
                DeadOnHelper.ForceNextDeadOn = true;

            await CardCmd.AutoPlay(ctx, drawnCards[0], null);
            return;
        }

        // Let the player choose one card from the drawn cards
        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1);
        var selected = (await CardSelectCmd.FromSimpleGrid(
            ctx,
            drawnCards,
            Owner,
            prefs
        )).FirstOrDefault();

        if (selected == null)
            return;

        // If Dead On triggered, mark the selected card so its Dead On also triggers
        if (DeadOnHelper.IsDeadOn)
        {
            DeadOnHelper.ForceNextDeadOn = true;
        }

        // Auto-play the selected card (it's already in hand)
        await CardCmd.AutoPlay(ctx, selected, null);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(UpgradedCardCount - CardCount);
    }
}
