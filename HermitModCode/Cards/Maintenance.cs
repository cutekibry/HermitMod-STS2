using HermitMod.Cards;
using HermitMod.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace HermitMod.Cards;

/// <summary>
/// Gain 1 Dexterity. This costs 1 more this combat.
/// Upgrade: Gain 2 Dexterity.
/// </summary>
public sealed class Maintenance : HermitCard
{
    private const int StrikeDmg = 3;
    private const int UpgradedStrikeDmg = 4;
    private const int DexAmount = 1;
    private const int UpgradedDexAmount = 2;

    public Maintenance() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.None) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<MaintenanceStrikePower>((decimal)StrikeDmg),
        new PowerVar<DexterityPower>((decimal)DexAmount)
    ];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<DexterityPower>()];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        int strikeDmg = IsUpgraded ? UpgradedStrikeDmg : StrikeDmg;
        int dex = IsUpgraded ? UpgradedDexAmount : DexAmount;
        await PowerCmd.Apply<MaintenanceStrikePower>(ctx, Owner.Creature, strikeDmg, Owner.Creature, this);
        await PowerCmd.Apply<DexterityPower>(ctx, Owner.Creature, dex, Owner.Creature, this);

        // This card costs 1 more each time it's played this combat
        EnergyCost.UpgradeBy(1);
        EnergyCost.FinalizeUpgrade();
    }

    protected override void OnUpgrade()
    {
        DynamicVars["MaintenanceStrikePower"].UpgradeValueBy(UpgradedStrikeDmg - StrikeDmg);
        DynamicVars["DexterityPower"].UpgradeValueBy(UpgradedDexAmount - DexAmount);
    }
}
