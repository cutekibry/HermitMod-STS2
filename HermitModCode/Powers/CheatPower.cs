using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace HermitMod.Powers;

/// <summary>
/// Cheat: Your next card will trigger its Dead On effect. No matter if it has Dead On or not, the power will be consumed after one card is played.
/// </summary>
public sealed class CheatPower : HermitPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Remove(this);
    }
}
