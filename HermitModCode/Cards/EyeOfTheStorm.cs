using HermitMod.Character;
using HermitMod.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace HermitMod.Cards;

/// <summary>
/// Concentrate. Gain energy to maximum. Exhaust.
/// Upgrade: Cost reduced from 1 to 0.
/// </summary>
public sealed class EyeOfTheStorm : HermitCard
{
    public EyeOfTheStorm() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.None) { }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<ConcentrationPower>()];

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<ConcentrationPower>(ctx, Owner.Creature, 1, Owner.Creature, this);

        int gain = Owner.PlayerCombatState!.MaxEnergy - Owner.PlayerCombatState.Energy;
        if (gain > 0)
            await PlayerCmd.GainEnergy(gain, Owner);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1); // 1 -> 0
        EnergyCost.FinalizeUpgrade();
    }
}
