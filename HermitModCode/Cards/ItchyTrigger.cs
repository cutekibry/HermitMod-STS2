using HermitMod.Cards;
using HermitMod.Patches;
using HermitMod.Utility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// Deal 7 damage. Dead On: Reduce the cost of a random card in your hand by 1 this turn.
/// Upgrade: 9 damage and reduce the cost by 2.
/// </summary>
public sealed class ItchyTrigger : HermitCard
{
    public override bool HasDeadOn => true;

    private const int DamageAmount = 7;
    private const int UpgradedDamageAmount = 9;
    private const int CostReduction = 1;
    private const int UpgradedCostReduction = 2;

    public ItchyTrigger() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(DamageAmount, ValueProp.Move),
        new DynamicVar("CostReduction", CostReduction),
    ];

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        HermitSfx.PlayGun2();
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target!).WithHermitGunHitFx().Execute(ctx);
    }
    protected override Task AfterPlayInternalIfDeadOn(PlayerChoiceContext ctx, CardPlay play)
    {
        IReadOnlyList<CardModel> cards = PileType.Hand.GetPile(Owner).Cards;
        Rng combatCardSelection = Owner.RunState.Rng.CombatCardSelection;
        CardModel? cardModel = combatCardSelection.NextItem(cards.Where(c => c.CostsEnergyOrStars(includeGlobalModifiers: false)));
        if (cardModel == null)
        {
            combatCardSelection.NextItem(cards.Where((CardModel c) => c.CostsEnergyOrStars(includeGlobalModifiers: true)));
        }
        cardModel?.EnergyCost.AddThisTurnOrUntilPlayed(-(int)DynamicVars["CostReduction"].BaseValue, reduceOnly: true);
        return Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UpgradedDamageAmount - DamageAmount);
        DynamicVars["CostReduction"].UpgradeValueBy(UpgradedCostReduction - CostReduction);
    }
}
