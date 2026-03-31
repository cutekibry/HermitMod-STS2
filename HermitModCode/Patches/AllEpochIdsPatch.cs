using System;
using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Timeline;

namespace HermitMod.Patches;

[HarmonyPatch(typeof(EpochModel), nameof(EpochModel.AllEpochIds), MethodType.Getter)]
public static class AllEpochIdsPatch
{
    private static IReadOnlyList<string>? _patchedList;

    [HarmonyPostfix]
    public static void Postfix(ref IReadOnlyList<string> __result)
    {
        if (_patchedList != null)
        {
            __result = _patchedList;
            return;
        }

        var list = new List<string>(__result);
        bool changed = false;

        foreach (var type in EpochRegistration.HermitEpochTypes)
        {
            var epoch = (EpochModel)Activator.CreateInstance(type)!;
            if (!list.Contains(epoch.Id))
            {
                list.Add(epoch.Id);
                changed = true;
            }
        }

        if (changed)
        {
            _patchedList = list.AsReadOnly();
            __result = _patchedList;
        }
    }
}
