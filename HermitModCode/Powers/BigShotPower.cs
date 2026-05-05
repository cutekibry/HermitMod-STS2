using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace HermitMod.Powers;

/// <summary>
/// At the start of your turn, you can Exhaust a card to gain 8 Block.
/// Stacks increase block gained (8 per stack).
/// </summary>
public sealed class BigShotPower : HermitPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDeadOnTriggered(PlayerChoiceContext ctx, CardPlay? play)
    {
        await PowerCmd.Apply<VigorPower>(ctx, Owner, Amount, Owner, play?.Card);
    }

}
