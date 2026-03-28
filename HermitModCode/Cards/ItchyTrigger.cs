using HermitMod.Cards;
using HermitMod.Patches;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// Deal 8 damage. Dead On: Reduce the cost of a random card in your hand by 1 this turn.
/// Upgrade: 11 damage.
/// </summary>
public sealed class ItchyTrigger : HermitCard
{
    private const int DamageAmount = 8;
    private const int UpgradedDamageAmount = 11;

    public ItchyTrigger() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar((decimal)DamageAmount, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target).Execute(ctx);

        if (DeadOnHelper.IsDeadOn)
        {
            DeadOnHelper.IncrementDeadOnCount();

            // Reduce cost of a random card in hand by 1 this turn
            var handCards = PileType.Hand.GetPile(Owner).Cards
                .Where(c => !c.EnergyCost.CostsX && c != this)
                .ToList();

            if (handCards.Count > 0)
            {
                var rng = new Random();
                var target = handCards[rng.Next(handCards.Count)];
                target.EnergyCost.AddThisTurnOrUntilPlayed(-1, reduceOnly: true);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UpgradedDamageAmount - DamageAmount);
    }
}
