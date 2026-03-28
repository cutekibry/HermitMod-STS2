using HermitMod.Cards;
using HermitMod.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// Gain 2 Sniper Focus. (Single-target attacks deal 3 additional damage per stack.)
/// Upgrade: Gain 3 Sniper Focus.
/// </summary>
public sealed class SteadyAim : HermitCard
{
    private const int FocusAmt = 2;
    private const int UpgradedFocusAmt = 3;

    public SteadyAim() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<SniperFocusPower>((decimal)FocusAmt)];

    private int CurrentFocus => IsUpgraded ? UpgradedFocusAmt : FocusAmt;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<SniperFocusPower>(Owner.Creature, CurrentFocus, Owner.Creature, this);
    }

    protected override void OnUpgrade() { }
}
