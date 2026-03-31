using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Timeline;
using MegaCrit.Sts2.Core.Timeline.Epochs;

namespace HermitMod.Patches;

[HarmonyPatch(typeof(NeowEpoch), "GetTimelineExpansion")]
public static class NeowExpansionPatch
{
    [HarmonyPostfix]
    public static void Postfix(ref EpochModel[] __result)
    {
        var list = new List<EpochModel>(__result);

        foreach (var type in EpochRegistration.HermitEpochTypes)
        {
            var epoch = (EpochModel)Activator.CreateInstance(type)!;
            if (!list.Any(e => e.Id == epoch.Id))
                list.Add(epoch);
        }

        __result = list.ToArray();
    }
}
