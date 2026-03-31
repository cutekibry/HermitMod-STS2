using HermitMod.Cards;
using HermitMod.Utility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// Ethereal. Deal damage equal to your missing HP. Exhaust.
/// Upgrade: Remove Ethereal.
/// </summary>
public sealed class BlackWind : HermitCard
{
    public BlackWind() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(0m, ValueProp.Move)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal, CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        int missingHp = Owner.Creature.MaxHp - Owner.Creature.CurrentHp;
        await DamageCmd.Attack(missingHp)
            .FromCard(this)
            .Targeting(play.Target)
            .WithHermitFireHitFx()
            .Execute(ctx);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1); // 2 → 1
        EnergyCost.FinalizeUpgrade();
    }
}
