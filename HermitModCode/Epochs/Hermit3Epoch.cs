using System.Collections.Generic;
using HermitMod.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.Timeline;
using MegaCrit.Sts2.Core.Timeline;

namespace HermitMod.Epochs;

/// <summary>
/// Unlocked by completing Act 2 as the Hermit.
/// Unlocks 3 relics: Charred Glove, Spyglass, Black Powder.
/// </summary>
public class Hermit3Epoch : EpochModel
{
    public override string Id => "HERMITMOD-HERMIT3_EPOCH";
    public override EpochEra Era => EpochEra.Invitation3;
    public override int EraPosition => 1;
    public override string? StoryId => "Hermit";

    public static List<RelicModel> Relics => [
        ModelDb.Relic<CharredGlove>(),
        ModelDb.Relic<Spyglass>(),
        ModelDb.Relic<BlackPowder>()
    ];

    public override string UnlockText => CreateRelicUnlockText(Relics);

    public override void QueueUnlocks()
    {
        NTimelineScreen.Instance.QueueRelicUnlock(Relics);
    }
}
