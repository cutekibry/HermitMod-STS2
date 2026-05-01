using HermitMod.Utility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// Deal 4 damage to a random enemy 3 times. Shuffle a Doubt into your draw pile.
/// Upgrade: 5 damage.
/// </summary>
public sealed class SprayPray : HermitCard
{
    private const int DamageAmount = 4;
    private const int UpgradedDamageAmount = 5;
    private const int HitCount = 3;

    public SprayPray() : base(1, CardType.Attack, CardRarity.Common, TargetType.None) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar((decimal)DamageAmount, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromCard<Doubt>()];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        HermitSfx.PlayGun3();

        // Deal damage to a random enemy 3 times (each hit picks a random target)
        for (int i = 0; i < HitCount; i++)
        {
            var enemies = CombatState.HittableEnemies.ToList();
            if (enemies.Count == 0) break;

            Creature target = enemies[Rng.Chaotic.NextInt(enemies.Count)];
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Targeting(target)
                .WithHermitGunHitFx()
                .Execute(ctx);
        }

        // Shuffle a Doubt into the draw pile
        var doubt = CombatState.CreateCard<Doubt>(Owner);
        await CardPileCmd.AddGeneratedCardToCombat(doubt, PileType.Draw, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UpgradedDamageAmount - DamageAmount);
    }
}
