using HermitMod.Cards;
using HermitMod.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Cards;

public sealed class Combo : HermitCard
{
    public Combo() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<ComboPower>()];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        int amount = IsUpgraded ? 2 : 1;
        await PowerCmd.Apply<ComboPower>(Owner.Creature, amount, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        // Upgrade increases magic from 1 to 2 (handled in OnPlay)
    }
}
