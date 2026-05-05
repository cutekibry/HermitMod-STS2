using HermitMod.Cards;
using HermitMod.Patches;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Cards;

/// <summary>
/// Gain 7 Block. Dead On: Draw and Upgrade a card.
/// Upgrade: Gain 9 Block, Draw and Upgrade 2.
/// </summary>
public sealed class Vantage : HermitCard
{
    public override bool HasDeadOn => true;

    private const int BlockAmount = 7;
    private const int UpgradedBlockAmount = 9;
    private const int DrawUpgradeCount = 1;
    private const int UpgradedDrawUpgradeCount = 2;

    public Vantage() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(BlockAmount, ValueProp.Move),
        new CardsVar(DrawUpgradeCount)
    ];

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
    }
    protected override async Task AfterPlayInternalIfDeadOn(PlayerChoiceContext ctx, CardPlay play)
    {
        var drawn = await CardPileCmd.Draw(ctx, DynamicVars.Cards.BaseValue, Owner);
        foreach (var card in drawn)
            if (card != null && card.IsUpgradable)
                CardCmd.Upgrade(card);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(UpgradedBlockAmount - BlockAmount);
        DynamicVars.Cards.UpgradeValueBy(UpgradedDrawUpgradeCount - DrawUpgradeCount);
    }
}
