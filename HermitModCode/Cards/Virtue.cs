using HermitMod.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

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

    private int CurrentReduce => IsUpgraded ? UpgradedReduceAmount : ReduceAmount;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        var powers = Owner.Creature.Powers?.ToList();
        if (powers == null) return;

        foreach (var power in powers)
        {
            if (power.Type == PowerType.Debuff && power.Amount > 0)
            {
                if (power.Amount <= CurrentReduce)
                {
                    await PowerCmd.Remove(power);
                }
                else
                {
                    // Reduce the debuff by applying negative stacks
                    await PowerCmd.Apply(power, Owner.Creature, -CurrentReduce, Owner.Creature, this);
                }
            }
        }
    }

    protected override void OnUpgrade()
    {
        // 1 -> 2 reduction (handled by CurrentReduce)
    }
}
