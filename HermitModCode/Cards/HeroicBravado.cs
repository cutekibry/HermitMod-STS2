using HermitMod.Cards;
using HermitMod.Character;
using HermitMod.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Cards;

/// <summary>
/// Ethereal. Gain 1 Rugged. Reduce this card's cost by 2 this combat.
/// Upgrade: Reduce cost by 1 instead.
/// </summary>
public sealed class HeroicBravado : HermitCard
{
    private const int CostIncrease = 2;
    private const int UpgradedCostIncrease = 1;

    public HeroicBravado() : base(1, CardType.Skill, CardRarity.Rare, TargetType.None) { }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar("CostIncrease", CostIncrease)];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<RuggedPower>()];

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        // Increase this card's cost for rest of combat
        int increase = DynamicVars["CostIncrease"].IntValue;
        var newCost = Math.Max(0, (int)EnergyCost.GetWithModifiers(default) + increase);
        EnergyCost.SetCustomBaseCost(newCost);

        await PowerCmd.Apply<RuggedPower>(ctx, Owner.Creature, 1, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["CostIncrease"].UpgradeValueBy(UpgradedCostIncrease - CostIncrease);
    }
}
