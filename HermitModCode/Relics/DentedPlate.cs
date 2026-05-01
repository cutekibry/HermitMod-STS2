using HermitMod.Relics;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Relics;

/// <summary>
/// While your HP is at or below 50%, gain Energy and draw 1 card at the start of your turn.
/// </summary>
public sealed class DentedPlate : HermitRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override async Task BeforeSideTurnStart(PlayerChoiceContext ctx, CombatSide side, ICombatState combatState)
    {
        if (side != CombatSide.Player) return;

        // Only trigger when HP is at or below 50%
        var creature = Owner.Creature;
        if (creature.CurrentHp > creature.MaxHp / 2) return;

        Flash();
        await PlayerCmd.GainEnergy(1m, Owner);
        await CardPileCmd.Draw(ctx, 1, Owner, false);
    }
}
