using HermitMod.Cards;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Cards;

/// <summary>
/// Discard 1 card. Draw 1 card(s). Discarded Curses are Exhausted.
/// Upgrade: Draw 2.
/// </summary>
public sealed class Covet : HermitCard
{
    private const int DrawAmt = 1;
    private const int UpgradeDrawAmt = 1;

    public Covet() : base(0, CardType.Skill, CardRarity.Common, TargetType.None) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(DrawAmt)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        // Prompt to discard 1 card
        var selected = (await CardSelectCmd.FromHandForDiscard(
            ctx,
            Owner,
            new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 1),
            null,
            this
        )).FirstOrDefault();

        if (selected != null)
        {
            bool isCurse = selected.Type == CardType.Curse;

            if (isCurse)
            {
                // Exhaust curses instead of discarding
                await CardCmd.Exhaust(ctx, selected);
            }
            else
            {
                await CardCmd.Discard(ctx, selected);
            }
        }

        // Draw cards
        await CardPileCmd.Draw(ctx, DynamicVars.Cards.IntValue, Owner, false);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(UpgradeDrawAmt);
    }
}
