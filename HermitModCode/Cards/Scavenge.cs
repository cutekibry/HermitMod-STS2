using HermitMod.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// Gain 5 Block. Draw 1 card.
/// Upgrade: 8 Block, Draw 2.
/// </summary>
public sealed class Scavenge : HermitCard
{
    private const int BlockAmount = 5;
    private const int UpgradedBlockAmount = 8;
    private const int DrawAmount = 1;
    private const int UpgradedDrawAmount = 2;

    public Scavenge() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.None) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar((decimal)BlockAmount, ValueProp.Move),
        new CardsVar(DrawAmount)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        await CardPileCmd.Draw(ctx, DynamicVars.Cards.IntValue, Owner, false);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(UpgradedBlockAmount - BlockAmount);
        DynamicVars.Cards.UpgradeValueBy(UpgradedDrawAmount - DrawAmount);
    }
}
