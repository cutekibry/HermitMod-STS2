using HarmonyLib;
using HermitMod.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Potions;

namespace HermitMod.Patches;

[HarmonyPatch(typeof(NPotionPopup), "RefreshButtons")]
public static class ShotglassPatch
{
    private static readonly AccessTools.FieldRef<NPotionPopup, NPotionHolder> HolderRef =
        AccessTools.FieldRefAccess<NPotionPopup, NPotionHolder>("_holder");

    private static readonly AccessTools.FieldRef<NPotionPopup, NPotionPopupButton> UseButtonRef =
        AccessTools.FieldRefAccess<NPotionPopup, NPotionPopupButton>("_useButton");

    [HarmonyPostfix]
    public static void Postfix(NPotionPopup __instance)
    {
        DisableUseButtonIfShotglassIsSpent(__instance);
    }

    public static void DisableUseButtonIfShotglassIsSpent(NPotionPopup instance)
    {
        PotionModel? potion = HolderRef(instance)?.Potion?.Model;
        Shotglass? shotglass = potion?.Owner?.GetRelic<Shotglass>();

        if (shotglass != null && shotglass.IsInCombat && shotglass.AvailableUses == 0)
            UseButtonRef(instance)?.Disable();
    }
}

[HarmonyPatch(typeof(NPotionPopup), nameof(NPotionPopup._Ready))]
public static class ShotglassPotionPopupReadyPatch
{
    [HarmonyPostfix]
    public static void Postfix(NPotionPopup __instance)
    {
        ShotglassPatch.DisableUseButtonIfShotglassIsSpent(__instance);
    }
}
