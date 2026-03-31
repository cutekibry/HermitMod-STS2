using HermitMod.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Cards;

/// <summary>
/// Exhaust all Unplayable cards in hand. Gain 7 Block. Draw 1 card.
/// Upgrade: 10 Block, Draw 2.
/// </summary>
public sealed class Spite : HermitCard
{
    private const int BlockAmount = 8;
    private const int UpgradedBlockAmount = 8;
    private const int DrawAmount = 3;
    private const int UpgradedDrawAmount = 4;

    public Spite() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.None) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar((decimal)BlockAmount, ValueProp.Move), new CardsVar(DrawAmount)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        // Exhaust all Unplayable cards in hand
        var handPile = PileType.Hand.GetPile(Owner);
        if (handPile != null)
        {
            var unplayable = handPile.Cards
                .Where(c => c.Keywords.Contains(CardKeyword.Unplayable))
                .ToList();
            foreach (var card in unplayable)
            {
                await CardPileCmd.Add(card, PileType.Exhaust);
            }
        }

        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        await CardPileCmd.Draw(ctx, DynamicVars.Cards.IntValue, Owner, false);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(UpgradedBlockAmount - BlockAmount);
        DynamicVars.Cards.UpgradeValueBy(UpgradedDrawAmount - DrawAmount);
    }
}
