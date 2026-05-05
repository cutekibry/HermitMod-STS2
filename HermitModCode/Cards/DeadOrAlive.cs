using HermitMod.Character;
using HermitMod.Utility;
using HermitMod.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Cards;

namespace HermitMod.Cards;

/// <summary>
/// Deal {Damage} damage X times. If Fatal, get a Bounty. Exhaust.
/// Upgrade: 14 damage.
/// </summary>
public sealed class DeadOrAlive : HermitCard
{
    private const int DamageAmount = 8;
    private const int UpgradedDamageAmount = 11;

    private const int MonsterGoldAmount = 15;
    private const int EliteGoldAmount = 40;
    private const int BossGoldAmount = 100;

    public DeadOrAlive() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }

    protected override bool HasEnergyCostX => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(DamageAmount, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.Static(StaticHoverTip.Fatal), HoverTipFactory.FromKeyword(HermitKeywords.Bounty)];

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        int times = EnergyCost.CapturedXValue;

        for (int i = 0; i < times; i++)
        {
            await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target!).WithHermitBluntLightHitFx().Execute(ctx);

            // Stop if target died
            if (play.Target?.IsDead == true) break;
        }

        // If Fatal (target died), gain gold and track Bounty
        bool shouldTriggerFatal = play.Target!.Powers.All(p => p.ShouldOwnerDeathTriggerFatal());
        if (play.Target?.IsDead == true && shouldTriggerFatal)
        {
            AbstractRoom? currentRoom = Owner.Creature.CombatState?.RunState.CurrentRoom;

            ArgumentNullException.ThrowIfNull(currentRoom);
            var goldAmount = currentRoom.RoomType switch
            {
                RoomType.Monster => MonsterGoldAmount,
                RoomType.Elite => EliteGoldAmount,
                RoomType.Boss => BossGoldAmount,
                _ => throw new InvalidOperationException("Invalid room type for Dead Or Alive card."),
            };
            await PlayerCmd.GainGold(goldAmount, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UpgradedDamageAmount - DamageAmount);
    }
}
