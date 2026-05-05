using HermitMod.Character;
using HermitMod.Utility;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// Deal 14 damage. Costs 1 less per Strike or Defend played this turn.
/// Cost reduction is simplified - uses AfterCardPlayed to track.
/// </summary>
public sealed class ShortFuse : HermitCard
{
    private const int DamageAmount = 18;
    private const int UpgradedDamageAmount = 22;

    public ShortFuse() : base(3, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(DamageAmount, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromKeyword(HermitKeywords.Strike), HoverTipFactory.FromKeyword(HermitKeywords.Defend)];

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target!).WithHermitShortFuseHitFx().Execute(ctx);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UpgradedDamageAmount - DamageAmount);
    }


    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (card != this)
        {
            return Task.CompletedTask;
        }

        if (IsClone)
        {
            return Task.CompletedTask;
        }

        int amount = CombatManager.Instance.History.CardPlaysFinished.Count(e => (e.CardPlay.Card.Tags.Contains(CardTag.Strike) || e.CardPlay.Card.Tags.Contains(CardTag.Defend)) && e.CardPlay.Card.Owner == Owner && e.HappenedThisTurn(CombatState));
        ReduceCostBy(amount);
        return Task.CompletedTask;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner)
        {
            return Task.CompletedTask;
        }

        if (!cardPlay.Card.Tags.Contains(CardTag.Strike) && !cardPlay.Card.Tags.Contains(CardTag.Defend))
        {
            return Task.CompletedTask;
        }

        ReduceCostBy(1);
        return Task.CompletedTask;
    }

    private void ReduceCostBy(int amount)
    {
        EnergyCost.AddThisTurn(-amount);
    }
}
