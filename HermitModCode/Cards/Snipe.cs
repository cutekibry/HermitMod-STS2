using HermitMod.Cards;
using HermitMod.Character;
using HermitMod.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Cards;

/// <summary>
/// Your next Dead On effect this turn triggers twice. Exhaust.
/// Upgrade: Also gain 1 Concentrate.
/// </summary>
public sealed class Snipe : HermitCard
{
    public Snipe() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.None) { }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<SnipePower>()];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<SnipePower>(ctx, Owner.Creature, 1, Owner.Creature, this);

        if (IsUpgraded)
        {
            await PowerCmd.Apply<ConcentrationPower>(ctx, Owner.Creature, 1, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        // Upgrade adds Concentrate effect (handled in OnPlay)
    }
}
