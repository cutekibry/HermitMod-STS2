using HermitMod.Utility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// For each card in your exhaust pile, a random enemy loses 5 HP.
/// Upgrade: 7 HP per card.
/// </summary>
public sealed class FromBeyond : HermitCard
{
    private const int HpLossAmount = 5;
    private const int UpgradedHpLossAmount = 7;

    public FromBeyond() : base(1, CardType.Skill, CardRarity.Rare, TargetType.None) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new HpLossVar(HpLossAmount),
        new CalculationBaseVar(0),
        new CalculationExtraVar(1),
        new CalculatedVar("CalculatedHits").WithMultiplier(CountCardsInExhaust)
    ];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromKeyword(CardKeyword.Exhaust)];

    private static decimal CountCardsInExhaust(CardModel card, Creature? _)
    {
        var exhaustPile = PileType.Exhaust.GetPile(card.Owner);
        return exhaustPile?.Cards.Count ?? 0;
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        for (int i = 0; i < CountCardsInExhaust(this, null); i++)
        {
            Creature? enemy = Owner.RunState.Rng.CombatTargets.NextItem(CombatState!.HittableEnemies);
            if (enemy == null)
            {
                break;
            }

            HermitCombatFx.GroundFireOnTarget(enemy);
            await CreatureCmd.Damage(ctx, enemy, DynamicVars["HpLoss"].BaseValue, ValueProp.Unblockable | ValueProp.Unpowered, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["HpLoss"].UpgradeValueBy(UpgradedHpLossAmount - HpLossAmount);
    }
}
