using System.Collections.Generic;
using HermitMod.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.Timeline;
using MegaCrit.Sts2.Core.Timeline;

namespace HermitMod.Epochs;

/// <summary>
/// Unlocked by completing Ascension 1 as the Hermit.
/// Unlocks 3 cards: Golden Bullet, Determination, Black Wind.
/// </summary>
public class Hermit7Epoch : EpochModel
{
    public override string Id => "HERMITMOD-HERMIT7_EPOCH";
    public override EpochEra Era => EpochEra.Invitation5;
    public override int EraPosition => 1;
    public override string? StoryId => "Hermit";

    public static List<CardModel> Cards => [
        ModelDb.Card<GoldenBullet>(),
        ModelDb.Card<Determination>(),
        ModelDb.Card<BlackWind>()
    ];

    public override string UnlockText => CreateCardUnlockText(Cards);

    public override void QueueUnlocks()
    {
        NTimelineScreen.Instance.QueueCardUnlock(Cards);
    }
}
