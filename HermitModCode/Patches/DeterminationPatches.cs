using System.Threading.Tasks;
using HarmonyLib;
using HermitMod.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace HermitMod.Patches;

/// <summary>
/// When any power's amount changes on a creature with DeterminationPower,
/// and that power is a debuff that increased, grant Strength.
/// </summary>
[HarmonyPatch(typeof(Hook), nameof(Hook.AfterPowerAmountChanged))]
public static class DeterminationPatch
{
    [HarmonyPostfix]
    public static async Task Postfix(Task __result, PowerModel power, int previousAmount)
    {
        await __result;

        if (power == null) return;
        if (power.Type != PowerType.Debuff) return;
        if (power.Amount <= previousAmount) return;

        var owner = power.Owner;
        if (owner == null) return;

        var determination = owner.GetPower<DeterminationPower>();
        if (determination == null) return;

        await PowerCmd.Apply<StrengthPower>(owner, determination.Amount, owner, null);
    }
}
