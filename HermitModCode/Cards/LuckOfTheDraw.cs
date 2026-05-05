using HermitMod.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

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

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar("Threshold", CostThreshold)];

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        int threshold = DynamicVars["Threshold"].IntValue;
        int totalCost = 0;

        while (totalCost < threshold && PileType.Hand.GetPile(Owner)!.Cards.Count < 10)
        {
            var cards = (await CardPileCmd.Draw(ctx, 1, Owner)).ToList();
            if(cards.Count == 0)
                break;

            var card = cards[0];
            totalCost += card.EnergyCost.GetWithModifiers(CostModifiers.Local);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Threshold"].UpgradeValueBy(UpgradedCostThreshold - CostThreshold);
    }
}
