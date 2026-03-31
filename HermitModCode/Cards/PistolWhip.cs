using HermitMod.Cards;
using HermitMod.Character;
using HermitMod.Utility;
using HermitMod.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

public class PistolWhip() : HermitCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    private const int Dmg = 6;
    private const int UpgradeDmg = 3;
    private const int BruiseAmt = 3;
    private const int UpgradedBruiseAmt = 5;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar((decimal)Dmg, ValueProp.Move), new PowerVar<BruisePower>((decimal)BruiseAmt)];

    protected override IEnumerable<CardKeyword> CustomKeywords => [HermitKeywords.Bruise];

    private int CurrentBruise => IsUpgraded ? UpgradedBruiseAmt : BruiseAmt;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target).WithHermitBluntLightHitFx().Execute(ctx);
        await PowerCmd.Apply<BruisePower>(play.Target, CurrentBruise, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UpgradeDmg);
    }
}
