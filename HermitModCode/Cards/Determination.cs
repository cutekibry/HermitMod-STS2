using HermitMod.Cards;
using HermitMod.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace HermitMod.Cards;

public sealed class Determination : HermitCard
{
    public Determination() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None) { }

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        IsUpgraded ? [CardKeyword.Innate] : [];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<DeterminationPower>(Owner.Creature, 1, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        // Upgrade adds Innate (handled by CanonicalKeywords)
    }
}
