using System.Collections.Generic;
using HermitMod.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.Timeline;
using MegaCrit.Sts2.Core.Timeline;

namespace HermitMod.Epochs;

/// <summary>
/// Unlocked by completing Act 3 as the Hermit.
/// Unlocks 3 cards: Eternal Form, Showdown, Dead Man's Hand.
/// </summary>
public class Hermit4Epoch : EpochModel
{
    public override string Id => "HERMITMOD-HERMIT4_EPOCH";
    public override EpochEra Era => EpochEra.Invitation4;
    public override int EraPosition => 0;
    public override string? StoryId => "Hermit";

    public static List<CardModel> Cards => [
        ModelDb.Card<EternalForm>(),
        ModelDb.Card<Showdown>(),
        ModelDb.Card<DeadMansHand>()
    ];

    public override string UnlockText => CreateCardUnlockText(Cards);

    public override void QueueUnlocks()
    {
        NTimelineScreen.Instance.QueueCardUnlock(Cards);
    }
}
