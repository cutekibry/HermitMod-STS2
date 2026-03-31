using HermitMod.Cards;
using HermitMod.Character;
using HermitMod.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Cards;

public sealed class Brawl : HermitCard
{
    public Brawl() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.None) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<BruisePower>(3m)];

    protected override IEnumerable<CardKeyword> CustomKeywords => [HermitKeywords.Bruise];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<BrawlPower>()];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<BrawlPower>(Owner.Creature, DynamicVars["BruisePower"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["BruisePower"].UpgradeValueBy(2m); // 3 → 5
    }
}
