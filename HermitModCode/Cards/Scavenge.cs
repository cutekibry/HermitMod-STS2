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
/// Gain 12 Block. Dead On: Gain 5 Gold. Exhaust.
/// Upgrade: 15 Block, 10 Gold.
/// (Original STS1: Plated Armor 4/5. Adapted to Block since Plated Armor doesn't exist in STS2.)
/// </summary>
public sealed class Scavenge : HermitCard
{
    public override bool HasDeadOn => true;

    private const int BlockAmount = 12;
    private const int UpgradedBlockAmount = 15;
    private const int GoldAmount = 5;
    private const int UpgradedGoldAmount = 10;

    public Scavenge() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.None) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar((decimal)BlockAmount, ValueProp.Move)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);

        if (DeadOnHelper.IsDeadOn)
        {
            DeadOnHelper.IncrementDeadOnCount();
            int gold = IsUpgraded ? UpgradedGoldAmount : GoldAmount;
            await PlayerCmd.GainGold(gold, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(UpgradedBlockAmount - BlockAmount);
    }
}
