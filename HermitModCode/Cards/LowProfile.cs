using HermitMod.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// Gain 7 Block. Gain additional Block equal to 4 for each debuff on you.
/// Upgrade: 9 Block, 5 per debuff.
/// </summary>
public sealed class LowProfile : HermitCard
{
    private const int BlockAmount = 7;
    private const int UpgradedBlockAmount = 9;
    private const int PerDebuffBlock = 4;
    private const int UpgradedPerDebuffBlock = 5;

    public LowProfile() : base(1, CardType.Skill, CardRarity.Common, TargetType.None) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar((decimal)BlockAmount, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        // Count debuffs on the player
        int debuffCount = Owner.Creature.Powers
            .Count(p => p.Type == PowerType.Debuff);

        int bonusBlock = (IsUpgraded ? UpgradedPerDebuffBlock : PerDebuffBlock) * debuffCount;

        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);

        if (bonusBlock > 0)
        {
            await CreatureCmd.GainBlock(Owner.Creature, (decimal)bonusBlock, ValueProp.Move, play);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(UpgradedBlockAmount - BlockAmount);
    }
}
