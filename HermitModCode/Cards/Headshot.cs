using HermitMod.Cards;
using HermitMod.Patches;
using HermitMod.Utility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

public class Headshot() : HermitCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    public override bool HasDeadOn => true;

    private const int Dmg = 7;
    private const int UpgradeDmg = 2;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar((decimal)Dmg, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        HermitSfx.PlayGun2();

        int damage = DynamicVars.Damage.IntValue;
        if (DeadOnHelper.IsDeadOn)
        {
            damage *= 2;
        }

        await DamageCmd.Attack(damage).FromCard(this).Targeting(play.Target).WithHermitGunHitFx().Execute(ctx);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UpgradeDmg);
    }
}
