using System.Collections.Generic;
using HermitMod.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.Timeline;
using MegaCrit.Sts2.Core.Timeline;

namespace HermitMod.Epochs;

/// <summary>
/// Unlocked by completing Act 1 as the Hermit.
/// Unlocks 3 cards: Cheat, Ghostly Presence, Tracking Shot.
/// </summary>
public class Hermit2Epoch : EpochModel
{
    public override string Id => "HERMITMOD-HERMIT2_EPOCH";
    public override EpochEra Era => EpochEra.Invitation3;
    public override int EraPosition => 0;
    public override string? StoryId => "Hermit";

    public static List<CardModel> Cards => [
        ModelDb.Card<Cheat>(),
        ModelDb.Card<GhostlyPresence>(),
        ModelDb.Card<TrackingShot>()
    ];

    public override string UnlockText => CreateCardUnlockText(Cards);

    public override void QueueUnlocks()
    {
        NTimelineScreen.Instance.QueueCardUnlock(Cards);
    }
}
