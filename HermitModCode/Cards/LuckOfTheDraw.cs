using HermitMod.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Cards;

/// <summary>
/// Draw cards until total cost drawn is 3 or more.
/// Upgrade: Until total cost is 4 or more.
/// </summary>
public sealed class LuckOfTheDraw : HermitCard
{
    private const int CostThreshold = 3;
    private const int UpgradedCostThreshold = 4;

    public LuckOfTheDraw() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    private int Threshold => IsUpgraded ? UpgradedCostThreshold : CostThreshold;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        int totalCost = 0;
        var drawPile = PileType.Draw.GetPile(Owner);

        while (totalCost < Threshold)
        {
            int handBefore = PileType.Hand.GetPile(Owner)?.Cards.Count ?? 0;
            await CardPileCmd.Draw(ctx, 1, Owner, false);
            int handAfter = PileType.Hand.GetPile(Owner)?.Cards.Count ?? 0;

            if (handAfter <= handBefore) break;

            var hand = PileType.Hand.GetPile(Owner);
            if (hand != null && hand.Cards.Count > 0)
            {
                var lastCard = hand.Cards[^1];
                int cardCost = lastCard.EnergyCost.GetWithModifiers(CostModifiers.None);
                totalCost += cardCost;
            }
        }
    }

    protected override void OnUpgrade()
    {
        // Upgrade changes threshold from 3 to 4 (reflected in description)
        EnergyCost.UpgradeBy(0); // Force description refresh
    }
}
