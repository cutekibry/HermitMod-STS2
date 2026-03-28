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
/// Gain 5 Block. ALL enemies gain 1 Bruise.
/// Upgrade: 8 Block, 2 Bruise.
/// </summary>
public sealed class DuckAndCover : HermitCard
{
    private const int Blk = 5;
    private const int UpgradedBlk = 8;
    private const int BruiseAmt = 1;
    private const int UpgradedBruiseAmt = 2;

    public DuckAndCover() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar((decimal)Blk, ValueProp.Move), new PowerVar<BruisePower>((decimal)BruiseAmt)];

    private int CurrentBruise => IsUpgraded ? UpgradedBruiseAmt : BruiseAmt;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);

        foreach (var enemy in CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<BruisePower>(enemy, CurrentBruise, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(UpgradedBlk - Blk);
    }
}
