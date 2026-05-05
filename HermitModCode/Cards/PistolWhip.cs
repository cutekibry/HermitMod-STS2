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
using MegaCrit.Sts2.Core.HoverTips;

namespace HermitMod.Cards;

public class PistolWhip() : HermitCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    private const int DamageAmount = 6;
    private const int UpgradedDamageAmount = 8;
    private const int BruiseAmount = 3;
    private const int UpgradedBruiseAmount = 5;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(DamageAmount, ValueProp.Move), new PowerVar<BruisePower>(BruiseAmount)];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<BruisePower>()];

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target).WithHermitBluntLightHitFx().Execute(ctx);
        int bruise = DynamicVars["BruisePower"].IntValue;
        await PowerCmd.Apply<BruisePower>(ctx, play.Target, bruise, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UpgradedDamageAmount - DamageAmount);
        DynamicVars["BruisePower"].UpgradeValueBy(UpgradedBruiseAmount - BruiseAmount);
    }
}
