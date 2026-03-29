using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Powers;

/// <summary>
/// Whenever a debuff is applied to you, gain X Strength.
/// Implementation is in DeterminationPatches.cs via Harmony hook.
/// </summary>
public sealed class DeterminationPower : HermitPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
}
