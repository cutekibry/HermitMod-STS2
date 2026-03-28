using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Powers;

/// <summary>
/// At the start of your turn, you can Exhaust a card to gain 8 Block.
/// Stacks increase block gained (8 per stack).
/// </summary>
public sealed class AdaptPower : HermitPower
{
    private const int BlockPerStack = 8;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side != CombatSide.Player) return;
        if (Owner?.Player == null) return;
        if (CombatManager.Instance.IsOverOrEnding) return;

        var hand = PileType.Hand.GetPile(Owner.Player);
        if (hand == null || !hand.Cards.Any()) return;

        // Prompt player to select a card to exhaust (optional — min 0, max 1)
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 0, 1);
        var selected = (await CardSelectCmd.FromHand(
            choiceContext,
            Owner.Player,
            prefs,
            null,
            this
        )).FirstOrDefault();

        if (selected != null)
        {
            await CardCmd.Exhaust(choiceContext, selected);
            int blockAmount = BlockPerStack * Amount;
            await CreatureCmd.GainBlock(Owner, blockAmount, default, null);
        }
    }
}
