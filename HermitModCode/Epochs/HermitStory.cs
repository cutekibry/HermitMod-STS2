using MegaCrit.Sts2.Core.Timeline;

namespace HermitMod.Epochs;

public sealed class HermitStory : StoryModel
{
    protected override string Id => "HERMIT";

    public override EpochModel[] Epochs =>
    [
        EpochModel.Get<Hermit4Epoch>(),
        EpochModel.Get<Hermit3Epoch>(),
        EpochModel.Get<Hermit2Epoch>(),
        EpochModel.Get<Hermit5Epoch>(),
        EpochModel.Get<Hermit6Epoch>(),
        EpochModel.Get<Hermit7Epoch>()
    ];
}
