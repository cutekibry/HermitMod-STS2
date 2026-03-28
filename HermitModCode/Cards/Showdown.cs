using HermitMod.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// Deal 12 damage. Play all Strikes in your hand.
/// Upgrade: 16 damage.
/// </summary>
public sealed class Showdown : HermitCard
{
    private const int DamageAmount = 12;
    private const int UpgradedDamageAmount = 16;

    public Showdown() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar((decimal)DamageAmount, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target).Execute(ctx);

        // Auto-play all Strikes in hand
        var strikes = PileType.Hand.GetPile(Owner).Cards
            .Where(c => c.Rarity == CardRarity.Basic && c.Type == CardType.Attack)
            .ToList();

        foreach (var strike in strikes)
        {
            if (play.Target?.IsDead == true) break;
            if (MegaCrit.Sts2.Core.Combat.CombatManager.Instance.IsOverOrEnding) break;
            await CardCmd.AutoPlay(ctx, strike, play.Target);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UpgradedDamageAmount - DamageAmount);
    }
}
