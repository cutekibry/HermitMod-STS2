using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Powers;

/// <summary>
/// At the start of your turn, apply X Bruise to ALL enemies.
/// </summary>
public sealed class BrawlPower : HermitPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player) return;
        Flash();

        foreach (Creature enemy in CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<BruisePower>(choiceContext, enemy, Amount, Owner, null);
        }
    }
}
