using HermitMod.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// Gain 8 Strength for this turn only. Exhaust.
/// Upgrade: Gain 12 Strength instead.
/// </summary>
public sealed class ScopeIn : HermitCard
{
    private const int StrAmt = 8;
    private const int UpgradedStrAmt = 12;

    public ScopeIn() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<StrengthPower>((decimal)StrAmt)];

    private int CurrentStr => IsUpgraded ? UpgradedStrAmt : StrAmt;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        // Grant temporary Strength (removed at end of turn via TemporaryStrengthPower)
        await PowerCmd.Apply<StrengthPower>(Owner.Creature, CurrentStr, Owner.Creature, this);
        await PowerCmd.Apply<TemporaryStrengthPower>(Owner.Creature, CurrentStr, Owner.Creature, this);
    }

    protected override void OnUpgrade() { }
}
