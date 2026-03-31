using System.Collections.Generic;
using HermitMod.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.Timeline;
using MegaCrit.Sts2.Core.Timeline;

namespace HermitMod.Epochs;

/// <summary>
/// Unlocked by defeating 15 elites as the Hermit.
/// Unlocks 3 cards: Shadow Cloak, Gambit, From Beyond.
/// </summary>
public class Hermit5Epoch : EpochModel
{
    public override string Id => "HERMITMOD-HERMIT5_EPOCH";
    public override EpochEra Era => EpochEra.Invitation4;
    public override int EraPosition => 1;
    public override string? StoryId => "Hermit";

    public static List<CardModel> Cards => [
        ModelDb.Card<ShadowCloak>(),
        ModelDb.Card<Gambit>(),
        ModelDb.Card<FromBeyond>()
    ];

    public override string UnlockText => CreateCardUnlockText(Cards);

    public override void QueueUnlocks()
    {
        NTimelineScreen.Instance.QueueCardUnlock(Cards);
    }
}
