using HermitMod.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Cards;

/// <summary>
/// Discard your hand. Draw 3 cards.
/// Upgrade: Cost reduced from 1 to 0.
/// </summary>
public sealed class DeadMansHand : HermitCard
{
    private const int DrawCount = 3;

    public DeadMansHand() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(DrawCount)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        // Discard entire hand
        var handCards = PileType.Hand.GetPile(Owner).Cards.ToList();
        if (handCards.Count > 0)
        {
            await CardCmd.Discard(ctx, handCards);
        }

        // Draw 3 cards
        await CardPileCmd.Draw(ctx, DrawCount, Owner, false);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1); // 1 → 0
        EnergyCost.FinalizeUpgrade();
    }
}
