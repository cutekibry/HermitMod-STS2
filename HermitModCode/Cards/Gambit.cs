using HermitMod.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Cards;

/// <summary>
/// Put 2 random Attacks from discard into hand. They cost 1 less this turn. Exhaust.
/// Upgrade: 3 attacks.
/// </summary>
public sealed class Gambit : HermitCard
{
    private const int CardCount = 2;
    private const int UpgradedCardCount = 3;

    public Gambit() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(CardCount)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        int count = DynamicVars.Cards.IntValue;

        // Get attacks from discard pile
        var discardCards = PileType.Discard.GetPile(Owner).Cards
            .Where(c => c.Type == CardType.Attack)
            .ToList();

        // Shuffle and take up to count
        var rng = new Random();
        var selected = discardCards.OrderBy(_ => rng.Next()).Take(count).ToList();

        foreach (var card in selected)
        {
            // Move from discard to hand and reduce cost by 1 this turn
            await CardPileCmd.Add(card, PileType.Hand);
            card.EnergyCost.AddThisTurnOrUntilPlayed(-1, reduceOnly: true);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(UpgradedCardCount - CardCount);
    }
}
