using HermitMod.Cards;
using HermitMod.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// Apply 2 Weak and 2 Bruise to ALL enemies.
/// Upgrade: 3 Weak and 3 Bruise.
/// </summary>
public sealed class SuppressiveFire : HermitCard
{
    private const int DebuffAmt = 2;
    private const int UpgradedDebuffAmt = 3;

    public SuppressiveFire() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<WeakPower>((decimal)DebuffAmt), new PowerVar<BruisePower>((decimal)DebuffAmt)];

    private int CurrentAmt => IsUpgraded ? UpgradedDebuffAmt : DebuffAmt;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        foreach (var enemy in CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<WeakPower>(enemy, CurrentAmt, Owner.Creature, this);
            await PowerCmd.Apply<BruisePower>(enemy, CurrentAmt, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade() { }
}
