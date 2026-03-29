using HermitMod.Patches;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Cards;

/// <summary>
/// Play one of the top 3 cards in your draw pile.
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

        // Get the top N cards from the draw pile
        var topCards = drawPile.Cards.Take(CurrentCardCount).ToList();

        if (topCards.Count == 0)
            return;

        // Let the player choose one card
        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1);
        var selected = (await CardSelectCmd.FromSimpleGrid(
            ctx,
            topCards,
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

        // Play the selected card automatically
        await CardCmd.AutoPlay(ctx, selected, null);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(UpgradedCardCount - CardCount);
    }
}
