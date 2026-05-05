using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;

namespace HermitMod.Relics;

/// <summary>
/// Whenever you defeat an Elite encounter, heal 7 HP and gain 35 gold.
/// </summary>
public sealed class BrokenTooth : HermitRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    private const int HealAmount = 7;
    private const int GoldAmount = 35;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new HealVar(HealAmount),
        new GoldVar(GoldAmount),
    ];
    

    public override async Task AfterCombatVictory(CombatRoom room)
    {
        if(room.RoomType == RoomType.Elite)
        {
            Flash();
            await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
            await PlayerCmd.GainGold(DynamicVars.Gold.BaseValue, Owner);
        }
    }
}
