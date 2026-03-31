using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using HermitMod.Character;
using HermitMod.Epochs;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Saves.Managers;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.Timeline;

namespace HermitMod.Patches;

/// <summary>
/// Patches the character-specific epoch checks to handle the Hermit.
/// BaseLib blocks ObtainCharUnlockEpoch for custom characters, so we need our own
/// postfix to handle epochs 2-4 (act completions).
/// Epochs 5, 6, 7 need explicit patches since the base game uses hardcoded character checks.
/// </summary>
[HarmonyPatch]
public static class EpochUnlockPatches
{
    private static readonly MethodInfo? TryObtainMidRun = AccessTools.Method(
        typeof(ProgressSaveManager), "TryObtainEpochMidRun", (Type[])null, (Type[])null);

    private static readonly MethodInfo? TryObtainPostRun = AccessTools.Method(
        typeof(ProgressSaveManager), "TryObtainEpochPostRun", (Type[])null, (Type[])null);

    [HarmonyPatch(typeof(ProgressSaveManager), "ObtainCharUnlockEpoch")]
    [HarmonyPostfix]
    public static void CharUnlockEpochPostfix(ProgressSaveManager __instance, Player localPlayer, int act)
    {
        if (!(localPlayer.Character is Hermit))
            return;

        try
        {
            EpochModel epoch = act switch
            {
                0 => EpochModel.Get(EpochModel.GetId<Hermit2Epoch>()),
                1 => EpochModel.Get(EpochModel.GetId<Hermit3Epoch>()),
                2 => EpochModel.Get(EpochModel.GetId<Hermit4Epoch>()),
                _ => null
            };

            if (epoch != null)
            {
                TryObtainMidRun?.Invoke(__instance, new object[2] { epoch, localPlayer });
                SaveManager.Instance?.UnlockSlot(epoch.Id);
                MainFile.Logger.Info($"Hermit epoch obtained for completing Act {act + 1}");
            }
        }
        catch (Exception ex)
        {
            MainFile.Logger.Info($"Hermit act epoch check: {ex.Message}");
        }
    }

    [HarmonyPatch(typeof(ProgressSaveManager), "CheckFifteenElitesDefeatedEpoch")]
    [HarmonyPostfix]
    public static void EliteEpochPostfix(ProgressSaveManager __instance, Player localPlayer)
    {
        if (!(localPlayer.Character is Hermit))
            return;

        try
        {
            EpochModel epoch = EpochModel.Get(EpochModel.GetId<Hermit5Epoch>());
            HashSet<ModelId> eliteEncounterIds = GetEliteEncounterIds();
            ProgressState? progress = SaveManager.Instance?.Progress;
            if (progress == null) return;
            int eliteWins = 0;

            foreach (EncounterStats stats in progress.EncounterStats.Values)
            {
                if (!eliteEncounterIds.Contains(stats.Id))
                    continue;

                foreach (FightStats fightStat in stats.FightStats)
                {
                    if (fightStat.Character == ((AbstractModel)localPlayer.Character).Id)
                    {
                        eliteWins += fightStat.Wins;
                        break;
                    }
                }
            }

            if (eliteWins >= 15)
            {
                TryObtainMidRun?.Invoke(__instance, new object[2] { epoch, localPlayer });
                SaveManager.Instance?.UnlockSlot(epoch.Id);
            }
        }
        catch (Exception ex)
        {
            MainFile.Logger.Info($"Hermit elite epoch check: {ex.Message}");
        }
    }

    [HarmonyPatch(typeof(ProgressSaveManager), "CheckFifteenBossesDefeatedEpoch")]
    [HarmonyPostfix]
    public static void BossEpochPostfix(ProgressSaveManager __instance, Player localPlayer)
    {
        if (!(localPlayer.Character is Hermit))
            return;

        try
        {
            EpochModel epoch = EpochModel.Get(EpochModel.GetId<Hermit6Epoch>());
            HashSet<ModelId> bossEncounters = ModelDb.Acts
                .SelectMany((ActModel a) => a.AllBossEncounters.Select((EncounterModel e) => ((AbstractModel)e).Id))
                .ToHashSet();

            ProgressState? progress = SaveManager.Instance?.Progress;
            if (progress == null) return;
            int bossWins = 0;

            foreach (EncounterStats stats in progress.EncounterStats.Values)
            {
                if (!bossEncounters.Contains(stats.Id))
                    continue;

                foreach (FightStats fightStat in stats.FightStats)
                {
                    if (fightStat.Character == ((AbstractModel)localPlayer.Character).Id)
                    {
                        bossWins += fightStat.Wins;
                        break;
                    }
                }
            }

            if (bossWins >= 15)
            {
                TryObtainMidRun?.Invoke(__instance, new object[2] { epoch, localPlayer });
                SaveManager.Instance?.UnlockSlot(epoch.Id);
            }
        }
        catch (Exception ex)
        {
            MainFile.Logger.Info($"Hermit boss epoch check: {ex.Message}");
        }
    }

    [HarmonyPatch(typeof(ProgressSaveManager), "CheckAscensionOneCompleted")]
    [HarmonyPostfix]
    public static void AscensionEpochPostfix(ProgressSaveManager __instance,
        SerializablePlayer serializablePlayer, int ascension)
    {
        if (ascension != 1)
            return;

        try
        {
            CharacterModel charModel = ModelDb.GetById<CharacterModel>(serializablePlayer.CharacterId);
            if (charModel is Hermit)
            {
                EpochModel epoch = EpochModel.Get(EpochModel.GetId<Hermit7Epoch>());
                TryObtainPostRun?.Invoke(__instance, new object[2] { epoch, serializablePlayer });
                SaveManager.Instance?.UnlockSlot(epoch.Id);
            }
        }
        catch (Exception ex)
        {
            MainFile.Logger.Info($"Hermit ascension epoch check: {ex.Message}");
        }
    }

    [HarmonyPatch(typeof(SaveManager), "GetCardUnlockEpochIds")]
    [HarmonyPostfix]
    public static void CardEpochIdsPostfix(ref string[] __result)
    {
        try
        {
            List<string> list = new List<string>(__result)
            {
                EpochModel.GetId<Hermit2Epoch>(),
                EpochModel.GetId<Hermit4Epoch>(),
                EpochModel.GetId<Hermit5Epoch>(),
                EpochModel.GetId<Hermit7Epoch>()
            };
            __result = list.ToArray();
        }
        catch (Exception ex)
        {
            MainFile.Logger.Info($"Card epoch IDs patch: {ex.Message}");
        }
    }

    [HarmonyPatch(typeof(SaveManager), "GetRelicUnlockEpochIds")]
    [HarmonyPostfix]
    public static void RelicEpochIdsPostfix(ref string[] __result)
    {
        try
        {
            List<string> list = new List<string>(__result)
            {
                EpochModel.GetId<Hermit3Epoch>(),
                EpochModel.GetId<Hermit6Epoch>()
            };
            __result = list.ToArray();
        }
        catch (Exception ex)
        {
            MainFile.Logger.Info($"Relic epoch IDs patch: {ex.Message}");
        }
    }

    private static HashSet<ModelId> GetEliteEncounterIds()
    {
        try
        {
            MethodInfo method = AccessTools.Method(typeof(ProgressSaveManager), "GetEliteEncounters", (Type[])null, (Type[])null);
            if (method != null)
            {
                return (HashSet<ModelId>)method.Invoke(null, null);
            }
        }
        catch
        {
        }

        return ModelDb.Acts
            .SelectMany((ActModel a) => a.AllEliteEncounters.Select((EncounterModel e) => ((AbstractModel)e).Id))
            .ToHashSet();
    }
}
