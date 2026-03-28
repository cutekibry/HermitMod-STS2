using System.Linq;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Powers;

/// <summary>
/// At the start of your turn, deal [Amount] damage to ALL enemies.
/// </summary>
public sealed class SprayPatternPower : HermitPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, MegaCrit.Sts2.Core.Entities.Players.Player player)
    {
        if (player.Creature != Owner) return;

        var combatState = Owner.CombatState;
        if (combatState == null) return;

        await CreatureCmd.TriggerAnim(Owner, "Attack", 0.3f);

        foreach (var enemy in combatState.HittableEnemies.ToList())
        {
            await CreatureCmd.Damage(choiceContext, enemy, Amount,
                ValueProp.Move, Owner, null);
        }
    }
}
