using HermitMod.Cards;
using HermitMod.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Cards;

/// <summary>
/// Gain 3 Energy. Draw 3 cards. Whenever you end your turn with 0 Energy, lose 4 HP.
/// Upgrade: Draw 4 cards. Lose 3 HP instead.
/// </summary>
public sealed class OverwhelmingPower : HermitCard
{
    private const int DrawCount = 3;
    private const int UpgradedDrawCount = 4;
    private const int HpLossAmount = 4;
    private const int UpgradedHpLossAmount = 3;
    private const int EnergyGain = 3;

    public OverwhelmingPower() : base(1, CardType.Power, CardRarity.Rare, TargetType.None) { }

    private int CurrentDraw => IsUpgraded ? UpgradedDrawCount : DrawCount;
    private int CurrentHpLoss => IsUpgraded ? UpgradedHpLossAmount : HpLossAmount;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(EnergyGain), new CardsVar(DrawCount), new HpLossVar(HpLossAmount)];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<OverwhelmingPowerPower>()];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        // Gain energy
        await PlayerCmd.GainEnergy(EnergyGain, Owner);

        // Draw cards
        await CardPileCmd.Draw(ctx, CurrentDraw, Owner, false);

        // Apply the debuff: lose HP when ending turn with 0 energy
        await PowerCmd.Apply<OverwhelmingPowerPower>(ctx, Owner.Creature, CurrentHpLoss, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(UpgradedDrawCount - DrawCount);
        DynamicVars["HpLoss"].UpgradeValueBy(UpgradedHpLossAmount - HpLossAmount);
    }
}
