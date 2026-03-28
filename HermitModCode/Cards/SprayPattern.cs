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
/// Power: At the start of your turn, deal 3 damage to ALL enemies.
/// Upgrade: 5 damage.
/// </summary>
public sealed class SprayPattern : HermitCard
{
    private const int DmgAmt = 3;
    private const int UpgradedDmgAmt = 5;

    public SprayPattern() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar((decimal)DmgAmt, ValueProp.Move)];

    private int CurrentDmg => IsUpgraded ? UpgradedDmgAmt : DmgAmt;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<SprayPatternPower>(Owner.Creature, CurrentDmg, Owner.Creature, this);
    }

    protected override void OnUpgrade() { }
}
