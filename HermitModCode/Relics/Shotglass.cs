using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace HermitMod.Relics;

/// <summary>
/// First 2 times you use a potion each combat, gain a random potion.
/// You can only use 2 potions each combat.
/// </summary>
public sealed class Shotglass : HermitRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    private const int Limit = 2;

    public int AvailableUses { get; private set; } = 0;
    public bool IsInCombat { get; private set; } = false;

    public override bool ShowCounter => IsInCombat;
    public override int DisplayAmount => AvailableUses;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("Limit", Limit)
    ];

    public override Task BeforeCombatStart()
    {
        AvailableUses = (int)DynamicVars["Limit"].BaseValue;
        IsInCombat = true;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override async Task AfterPotionUsed(PotionModel potion, Creature? target)
    {
        if(potion.Owner != Owner || AvailableUses == 0 || !IsInCombat) 
            return;
        AvailableUses--;
        Flash();
        await PotionCmd.TryToProcure(PotionFactory.CreateRandomPotionInCombat(Owner, Owner.RunState.Rng.CombatPotionGeneration).ToMutable(), Owner);
        InvokeDisplayAmountChanged();

        if (AvailableUses == 0) 
            Status = RelicStatus.Disabled;
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        IsInCombat = false;
        Status = RelicStatus.Normal;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
}
