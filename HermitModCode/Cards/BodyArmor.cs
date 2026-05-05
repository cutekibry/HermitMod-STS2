using HermitMod.Cards;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// Discard a card. Gain 7 Block. If it was a non-Attack, gain 7 Block again.
/// Upgrade: 10 Block.
/// </summary>
public sealed class BodyArmor : HermitCard
{
    private const int Blk = 5;
    private const int UpgradeBlk = 2;

    public BodyArmor() : base(1, CardType.Skill, CardRarity.Common, TargetType.None) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar((decimal)Blk, ValueProp.Move)];

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        // Prompt player to discard a card
        var selected = (await CardSelectCmd.FromHandForDiscard(
            ctx,
            Owner,
            new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 1),
            null,
            this
        )).FirstOrDefault();

        if (selected != null)
        {
            bool wasNonAttack = selected.Type != CardType.Attack;
            await CardCmd.Discard(ctx, selected);

            await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);

            // If non-Attack was discarded, gain Block again
            if (wasNonAttack)
            {
                await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(UpgradeBlk);
    }
}
