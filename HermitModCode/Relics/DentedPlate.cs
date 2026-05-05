using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;

namespace HermitMod.Relics;

/// <summary>
/// While your HP is at or below 50%, gain Energy and draw 1 card at the start of your turn.
/// </summary>
public sealed class DentedPlate : HermitRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];

    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        if(player != Owner || player.Creature.CurrentHp > player.Creature.MaxHp / 2)
            return count;
        return count + 1;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext ctx, Player player)
    {
        if (player != Owner)
            return;
        
        if (player.Creature.CurrentHp <= player.Creature.MaxHp / 2) {
            Status = RelicStatus.Active;
            Flash();
            await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, player);
        }
        else
            Status = RelicStatus.Normal;
    }
}
