using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace HermitMod.Powers;

public sealed class FatalDesirePower : HermitPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player) return;

        Flash();
        await CardPileCmd.Draw(choiceContext, 2, Owner.Player!, false);
        var injury = CombatState.CreateCard<Injury>(Owner.Player!);
        await CardPileCmd.AddGeneratedCardToCombat(
            injury,
            PileType.Hand,
            Owner.Player
        );
    }
}
