using HermitMod.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Cards;

/// <summary>
/// Choose a card in your hand. It costs 0 this turn. Discard the rest of your hand.
/// Upgrade: Cost reduced from 1 to 0.
/// </summary>
public sealed class LoneWolf : HermitCard
{
    public LoneWolf() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.None) { }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        var handPile = PileType.Hand.GetPile(Owner);
        if (handPile == null || handPile.Cards.Count == 0) return;

        // Let player choose a card from hand
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 1);
        var chosen = await CardSelectCmd.FromHand(ctx, Owner, prefs, null, this);
        if (chosen != null && chosen.Any())
        {
            var chosenCard = chosen.First();
            // Make the chosen card cost 0 this turn
            chosenCard.EnergyCost.SetUntilPlayed(0, false);

            // Discard the rest of the hand
            var toDiscard = handPile.Cards.Where(c => c != chosenCard).ToList();
            foreach (var card in toDiscard)
            {
                await CardPileCmd.Add(card, PileType.Discard);
            }
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
        EnergyCost.FinalizeUpgrade();
    }
}
