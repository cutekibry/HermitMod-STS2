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
/// Deal 6 damage to ALL enemies. Deals 2 more for each Curse card in all piles.
/// Upgrade: 9 damage.
/// </summary>
public sealed class Grudge : HermitCard
{
    private const int DamageAmount = 9;
    private const int ExtraDamageAmount = 2;
    private const int UpgradedExtraDamageAmount = 3;

    public Grudge() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(DamageAmount, ValueProp.Move),
        new CalculationBaseVar(DamageAmount),
        new ExtraDamageVar(ExtraDamageAmount),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(CountCurses)
    ];

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);

        await DamageCmd.Attack(DynamicVars.CalculatedDamage)
            .FromCard(this)
            .TargetingAllOpponents(CombatState!)
            .WithHermitFireHitFx()
            .Execute(ctx);
    }

    private static decimal CountCurses(CardModel card, Creature? _)
    {
        int curseCount = 0;
        foreach (var pileType in new[] { PileType.Draw, PileType.Hand, PileType.Discard })
        {
            var pile = pileType.GetPile(card.Owner);
            if (pile != null)
                curseCount += pile.Cards.Count(c => c.Type == CardType.Curse);
        }

        return curseCount;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.ExtraDamage.UpgradeValueBy(UpgradedExtraDamageAmount - ExtraDamageAmount);
    }
}
