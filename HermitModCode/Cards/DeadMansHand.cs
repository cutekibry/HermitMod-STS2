using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;

namespace HermitMod.Cards;

/// <summary>
/// Discard your hand. Add the 3 rarest cards from your draw pile to your hand.
/// Upgrade: Cost reduced from 1 to 0.
/// </summary>
public sealed class DeadMansHand : HermitCard
{
    private const int DrawCount = 3;

    public DeadMansHand() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(DrawCount)];

    private static int RarityLevel(CardRarity rarity) => rarity switch
    {
        CardRarity.Ancient => 3,
        CardRarity.Rare => 2,
        CardRarity.Uncommon => 1,
        _ => 0,
    };

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        // Discard entire hand
        var handCards = PileType.Hand.GetPile(Owner).Cards.ToList();
        if (handCards.Count > 0)
        {
            await CardCmd.Discard(ctx, handCards);
        }

        var drawCards = PileType.Draw.GetPile(Owner).Cards.ToList();
        if (drawCards.Count > 0)
        {
            for (int i = 0; i < DrawCount && drawCards.Count > 0; i++)
            {
                int selectedIndex = 0;
                int selectedRarity = RarityLevel(drawCards[0].Rarity);
                for (int j = 1; j < drawCards.Count; j++)
                {
                    int rarity = RarityLevel(drawCards[j].Rarity);
                    if (rarity >= selectedRarity)
                    {
                        selectedIndex = j;
                        selectedRarity = rarity;
                    }
                }

                var cardToDraw = drawCards[selectedIndex];
                drawCards.RemoveAt(selectedIndex);
                await CardPileCmd.Add(cardToDraw, PileType.Hand);
            }
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1); // 1 → 0
        EnergyCost.FinalizeUpgrade();
    }
}
