using HermitMod.Cards;
using HermitMod.Character;
using HermitMod.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Cards;

/// <summary>
/// Concentrate. Gain 3 Energy. Exhaust.
/// Upgrade: Cost reduced from 1 to 0.
/// </summary>
public sealed class EyeOfTheStorm : HermitCard
{
    public EyeOfTheStorm() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.None) { }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<CardKeyword> CustomKeywords => [HermitKeywords.Concentrate];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(3)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<ConcentrationPower>(ctx, Owner.Creature, 1, Owner.Creature, this);
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1); // 1 鈫?0
        EnergyCost.FinalizeUpgrade();
    }
}
