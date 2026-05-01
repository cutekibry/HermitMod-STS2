using HermitMod.Cards;
using HermitMod.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Cards;

public sealed class Combo : HermitCard
{
    public Combo() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<ComboPower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar("ComboPower", 1)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        int amount = DynamicVars["ComboPower"].IntValue;
        await PowerCmd.Apply<ComboPower>(ctx, Owner.Creature, amount, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ComboPower"].UpgradeValueBy(1);
    }
}
