using HermitMod.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace HermitMod.Cards;

/// <summary>
/// Retain. Reduce each debuff on you by 1.
/// Upgrade: Reduce by 2.
/// </summary>
public sealed class Virtue : HermitCard
{
    private const int ReduceAmount = 1;
    private const int UpgradedReduceAmount = 2;

    public Virtue() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar("Reduce", ReduceAmount)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        int reduceBy = DynamicVars["Reduce"].IntValue;
        var powers = Owner.Creature.Powers?.ToList();
        if (powers == null) return;

        foreach (var power in powers)
        {
            if (power.Type == PowerType.Debuff && power.Amount > 0)
            {
                if (power.Amount <= reduceBy)
                {
                    await PowerCmd.Remove(power);
                }
                else
                {
                    power.SetAmount(power.Amount - reduceBy);
                }
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Reduce"].UpgradeValueBy(UpgradedReduceAmount - ReduceAmount);
    }
}
