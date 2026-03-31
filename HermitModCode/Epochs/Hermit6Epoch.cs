using System.Collections.Generic;
using HermitMod.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.Timeline;
using MegaCrit.Sts2.Core.Timeline;

namespace HermitMod.Epochs;

/// <summary>
/// Unlocked by defeating 15 bosses as the Hermit.
/// Unlocks 3 relics: Pet Ghost, Clasped Locket, Broken Tooth.
/// </summary>
public class Hermit6Epoch : EpochModel
{
    public override string Id => "HERMITMOD-HERMIT6_EPOCH";
    public override EpochEra Era => EpochEra.Invitation5;
    public override int EraPosition => 0;
    public override string? StoryId => "Hermit";

    public static List<RelicModel> Relics => [
        ModelDb.Relic<PetGhost>(),
        ModelDb.Relic<ClaspedLocket>(),
        ModelDb.Relic<BrokenTooth>()
    ];

    public override string UnlockText => CreateRelicUnlockText(Relics);

    public override void QueueUnlocks()
    {
        NTimelineScreen.Instance.QueueRelicUnlock(Relics);
    }
}
