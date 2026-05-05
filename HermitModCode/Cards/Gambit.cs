using HermitMod.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Random;

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

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        int count = DynamicVars.Cards.IntValue;

        // Shuffle and take up to count
        for (int i = 0; i < DynamicVars.Cards.BaseValue; i++)
        {
            var discardedAttacks = PileType.Discard.GetPile(Owner).Cards.Where(c => c.Type == CardType.Attack);
            Rng combatCardSelection = Owner.RunState.Rng.CombatCardSelection;
            CardModel? card = combatCardSelection.NextItem(discardedAttacks);
            if (card == null) break;
            
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
