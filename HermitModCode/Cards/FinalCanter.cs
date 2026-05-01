using HermitMod.Cards;
using HermitMod.Utility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// Deal 10 damage. Deal 3 additional damage per Curse in all piles. Retain. Exhaust.
/// Upgrade: 13 damage, 4 per Curse.
/// </summary>
public sealed class FinalCanter : HermitCard
{
    private const int DamageAmount = 10;
    private const int UpgradedDamageAmount = 13;
    private const int ExtraDamage = 3;
    private const int UpgradedExtraDamage = 4;

    public FinalCanter() : base(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(DamageAmount),
        new ExtraDamageVar(ExtraDamage),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(CountCurses)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);

        await DamageCmd.Attack(DynamicVars.CalculatedDamage)
            .FromCard(this)
            .Targeting(play.Target!)
            .WithHermitFireHitFx()
            .Execute(ctx);
    }

    private static decimal CountCurses(CardModel card, Creature? _)
    {
        int curseCount = 0;
        foreach (var pileType in new[] { PileType.Draw, PileType.Hand, PileType.Discard, PileType.Exhaust })
        {
            var pile = pileType.GetPile(card.Owner);
            if (pile != null)
                curseCount += pile.Cards.Count(c => c.Type == CardType.Curse);
        }

        return curseCount;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(UpgradedDamageAmount - DamageAmount);
        DynamicVars.ExtraDamage.UpgradeValueBy(UpgradedExtraDamage - ExtraDamage);
    }
}
