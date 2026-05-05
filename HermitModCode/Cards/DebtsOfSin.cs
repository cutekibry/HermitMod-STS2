using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace HermitMod.Cards;

/// <summary>
/// Exhaust ALL Unplayable cards from other players. Add ALL exhausted curse cards to your hand.
/// Upgrade: Retain.
/// </summary>
public sealed class DebtsOfSin : HermitCard
{
    public DebtsOfSin() : base(1, CardType.Skill, CardRarity.Rare, TargetType.None) { }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromKeyword(CardKeyword.Unplayable)];

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        var players = CombatState!.GetTeammatesOf(Owner.Creature)
            .Where(c => c is { IsAlive: true, IsPlayer: true, Player: not null } && c.Player != Owner);

        foreach (Creature creature in players)
        {
            foreach (var pileType in new[] { PileType.Hand, PileType.Discard, PileType.Draw }) 
            {
                var unplayableCards = pileType.GetPile(creature.Player!).Cards
                    .Where(c => c.Keywords.Contains(CardKeyword.Unplayable))
                    .ToList();

                foreach (var card in unplayableCards)
                {
                    await CardCmd.Exhaust(ctx, card);
                    if (card.Type == CardType.Curse) {
                        var newCard = CombatState.CreateCard(card, Owner);
                        await CardPileCmd.AddGeneratedCardToCombat(newCard, PileType.Hand, Owner);
                    }
                }
            }
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
