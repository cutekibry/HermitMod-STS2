using HermitMod.Cards;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Cards;

/// <summary>
/// Gain 6 Block. Retain up to 2 cards this turn.
/// Upgrade: 9 Block, Retain 3.
/// </summary>
public sealed class Coalescence : HermitCard
{
    private const int BlockAmount = 6;
    private const int UpgradedBlockAmount = 9;
    private const int RetainCount = 2;
    private const int UpgradedRetainCount = 3;

    public Coalescence() : base(1, CardType.Skill, CardRarity.Common, TargetType.None) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar((decimal)BlockAmount, ValueProp.Move),
        new CardsVar(RetainCount)
    ];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);

        // Let the player choose up to N cards from hand to Retain this turn
        var hand = PileType.Hand.GetPile(Owner);
        if (hand == null || hand.Cards.Count == 0) return;

        var retainable = hand.Cards.Where(c => !c.ShouldRetainThisTurn).ToList();
        if (retainable.Count == 0) return;

        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 0, DynamicVars.Cards.IntValue);
        var selected = (await CardSelectCmd.FromHand(
            prefs: prefs,
            context: ctx,
            player: Owner,
            filter: c => !c.ShouldRetainThisTurn,
            source: this
        )).ToList();

        foreach (var card in selected)
        {
            card.GiveSingleTurnRetain();
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(UpgradedBlockAmount - BlockAmount);
        DynamicVars.Cards.UpgradeValueBy(UpgradedRetainCount - RetainCount);
    }
}
