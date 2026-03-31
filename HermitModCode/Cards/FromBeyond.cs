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
/// For each card in your exhaust pile, a random enemy loses 5 HP.
/// Upgrade: 7 HP per card.
/// </summary>
public sealed class FromBeyond : HermitCard
{
    private const int HpLossAmount = 5;
    private const int UpgradedHpLossAmount = 7;

    public FromBeyond() : base(1, CardType.Skill, CardRarity.Rare, TargetType.None) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new HpLossVar(HpLossAmount)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        var exhaustPile = PileType.Exhaust.GetPile(Owner);
        int exhaustCount = exhaustPile?.Cards.Count ?? 0;

        int hpLoss = IsUpgraded ? UpgradedHpLossAmount : HpLossAmount;
        var rng = new System.Random();

        for (int i = 0; i < exhaustCount; i++)
        {
            var enemies = CombatState.HittableEnemies;
            if (enemies.Count == 0) break;

            var target = enemies[rng.Next(enemies.Count)];
            HermitCombatFx.GroundFireOnTarget(target);
            await CreatureCmd.Damage(ctx, target, hpLoss,
                ValueProp.Unblockable | ValueProp.Unpowered, Owner.Creature, null);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["HpLoss"].UpgradeValueBy(UpgradedHpLossAmount - HpLossAmount);
    }
}
