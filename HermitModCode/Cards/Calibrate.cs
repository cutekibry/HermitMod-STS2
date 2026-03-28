using HermitMod.Cards;
using HermitMod.Patches;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// Gain 8 Block. Dead On: Apply 2 Vulnerable to an enemy.
/// Upgrade: 11 Block, 3 Vulnerable.
/// </summary>
public sealed class Calibrate : HermitCard
{
    private const int Blk = 8;
    private const int UpgradedBlk = 11;
    private const int VulnAmt = 2;
    private const int UpgradedVulnAmt = 3;

    public Calibrate() : base(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar((decimal)Blk, ValueProp.Move), new PowerVar<VulnerablePower>((decimal)VulnAmt)];

    private int CurrentVuln => IsUpgraded ? UpgradedVulnAmt : VulnAmt;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);

        if (DeadOnHelper.IsDeadOn)
        {
            DeadOnHelper.IncrementDeadOnCount();
            await PowerCmd.Apply<VulnerablePower>(play.Target, CurrentVuln, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(UpgradedBlk - Blk);
    }
}
