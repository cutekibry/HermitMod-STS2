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
/// Deal 6 damage to ALL enemies. Deals 2 more for each Curse card in all piles.
/// Upgrade: 9 damage.
/// </summary>
public sealed class Grudge : HermitCard
{
    private const int DamageAmount = 6;
    private const int UpgradedDamageAmount = 9;
    private const int BonusDamagePerCurse = 2;

    public Grudge() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar((decimal)DamageAmount, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);

        // Count all Curse cards across all piles
        int curseCount = 0;
        foreach (var pileType in new[] { PileType.Draw, PileType.Hand, PileType.Discard, PileType.Exhaust })
        {
            var pile = pileType.GetPile(Owner);
            if (pile != null)
            {
                curseCount += pile.Cards.Count(c => c.Type == CardType.Curse);
            }
        }

        decimal totalDamage = DynamicVars.Damage.BaseValue + (curseCount * BonusDamagePerCurse);

        await DamageCmd.Attack(totalDamage)
            .FromCard(this)
            .TargetingAllOpponents(CombatState)
            .WithHermitFireHitFx()
            .Execute(ctx);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UpgradedDamageAmount - DamageAmount);
    }
}
