using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Saves.Managers;
using MegaCrit.Sts2.Core.Timeline;

namespace HermitMod.Patches;

[HarmonyPatch(typeof(ProgressSaveManager), "LoadProgress")]
public static class EpochSlotInitializer
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        try
        {
            var saveManager = SaveManager.Instance;
            if (saveManager == null) return;

            var progress = saveManager.Progress;

            foreach (var type in EpochRegistration.HermitEpochTypes)
            {
                var epoch = (EpochModel)Activator.CreateInstance(type)!;
                var serialized = progress.Epochs.FirstOrDefault(e => e.Id == epoch.Id);

                // State 3 = ObtainedNoSlot — promote to full slot
                if (serialized != null && (int)serialized.State == 3)
                {
                    saveManager.UnlockSlot(epoch.Id);
                    MainFile.Logger.Info($"Promoted epoch {epoch.Id} from ObtainedNoSlot to Obtained.");
                }
            }
        }
        catch (Exception ex)
        {
            MainFile.Logger.Error($"Failed to check epoch slots: {ex}");
        }
    }
}
