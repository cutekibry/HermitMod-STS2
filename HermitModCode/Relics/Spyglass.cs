using HermitMod.Relics;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Relics;

/// <summary>
/// At the start of each turn, draw 1 extra card.
/// </summary>
public sealed class Spyglass : HermitRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override async Task BeforeSideTurnStart(PlayerChoiceContext ctx, CombatSide side, ICombatState combatState)
    {
        if (side != CombatSide.Player) return;
        Flash();
        await CardPileCmd.Draw(ctx, 1, Owner, false);
    }
}
