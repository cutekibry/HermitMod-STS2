using HermitMod.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace HermitMod.Cards;

/// <summary>
/// At the start of your turn, draw 2 cards and add an Injury to your hand.
/// Upgrade: Cost 0.
/// </summary>
public sealed class FatalDesire : HermitCard
{
    public FatalDesire() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        IsUpgraded ? [CardKeyword.Innate] : [];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromCard<Injury>()];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<FatalDesirePower>(Owner.Creature, 1, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        // Upgrade adds Innate (handled by CanonicalKeywords)
    }
}
