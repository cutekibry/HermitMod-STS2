using System;
using System.Reflection;
using Godot;
using HarmonyLib;
using HermitMod.Patches;
using MegaCrit.Sts2.Core.Modding;

namespace HermitMod;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "HermitMod";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        // Register custom Hermit epochs into the game's EpochModel system
        EpochRegistration.RegisterEpochs();

        Harmony harmony = new(ModId);

        // Patch each type individually so one failure doesn't block everything
        var assembly = Assembly.GetExecutingAssembly();
        foreach (var type in assembly.GetTypes())
        {
            try
            {
                var patchInfo = HarmonyMethodExtensions.GetFromType(type);
                if (patchInfo != null && patchInfo.Count > 0)
                {
                    harmony.CreateClassProcessor(type).Patch();
                }
            }
            catch (Exception ex)
            {
                Logger.Info($"Skipping patch {type.Name}: {ex.Message}");
            }
        }

        Logger.Info("The Hermit mod loaded successfully!");
    }
}
