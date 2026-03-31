using HermitMod.Cards;
using HermitMod.Patches;
using HermitMod.Utility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Cards;

public class Roughhouse() : HermitCard(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    public override bool HasDeadOn => true;

    private const int Dmg = 24;
    private const int UpgradeDmg = 6;
    private const int Blk = 20;
    private const int UpgradeBlk = 4;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar((decimal)Dmg, ValueProp.Move), new BlockVar((decimal)Blk, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target).WithHermitBluntHeavyHitFx().Execute(ctx);

        if (DeadOnHelper.IsDeadOn)
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UpgradeDmg);
        DynamicVars.Block.UpgradeValueBy(UpgradeBlk);
    }
}
