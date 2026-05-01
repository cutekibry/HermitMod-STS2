using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Rooms;
using HermitMod.Cards;

namespace HermitMod.Relics;

/// <summary>
/// Starter relic. At the start of each combat, add a Memento into your hand.
/// </summary>
public sealed class OldLocket : HermitRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    private bool _firstTurn = true;

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, ICombatState combatState)
    {
        if (!_firstTurn || side != Owner.Creature.Side) return;
        _firstTurn = false;

        Flash();
        var card = combatState.CreateCard<MementoCard>(Owner);
        await CardPileCmd.AddGeneratedCardToCombat(
            card,
            PileType.Hand,
            Owner
        );
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        _firstTurn = true;
        return Task.CompletedTask;
    }
}
