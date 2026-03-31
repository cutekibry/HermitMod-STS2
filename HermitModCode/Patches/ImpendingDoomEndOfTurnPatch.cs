using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using HermitMod.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Patches;

/// <summary>
/// Deals 13 damage to all creatures when ImpendingDoom is in the middle of the hand at end of turn.
/// </summary>
[HarmonyPatch(typeof(Hook), "BeforeTurnEnd")]
public static class ImpendingDoomEndOfTurnPatch
{
    private const int DoomDamage = 13;

    [HarmonyPostfix]
    public static async Task Postfix(Task __result, CombatState combatState, CombatSide side)
    {
        await __result;

        if ((int)side != 1 || combatState == null)
            return;

        foreach (Player player in combatState.Players)
        {
            if (player?.Creature?.IsDead == true)
                continue;

            CardPile handPile = PileTypeExtensions.GetPile(PileType.Hand, player);
            if (handPile == null)
                continue;

            IReadOnlyList<CardModel> handCards = handPile.Cards;
            if (handCards == null || handCards.Count == 0)
                continue;

            int handSize = handCards.Count;
            for (int i = 0; i < handCards.Count; i++)
            {
                if (handCards[i] is not ImpendingDoom || !DeadOnHelper.IsMiddlePosition(i, handSize))
                    continue;

                await CreatureCmd.TriggerAnim(player.Creature, "Cast", player.Character.CastAnimDelay);

                List<Creature> allTargets = combatState.HittableEnemies.ToList();
                allTargets.Add(player.Creature);

                ulong? netId = LocalContext.NetId;
                if (!netId.HasValue)
                    break;

                HookPlayerChoiceContext ctx = new HookPlayerChoiceContext(player, netId.Value, (GameActionType)1);

                foreach (Creature target in allTargets)
                {
                    if (target == null || !target.IsDead)
                    {
                        await CreatureCmd.Damage((PlayerChoiceContext)(object)ctx, target, DoomDamage, (ValueProp)8, player.Creature, handCards[i]);
                    }
                }
                break;
            }
        }
    }
}
