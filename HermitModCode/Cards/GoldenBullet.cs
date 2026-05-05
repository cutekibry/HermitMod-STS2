using HermitMod.Utility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// Deal 20 damage. If Fatal, permanently reduce this card's cost by 1. Exhaust.
/// Upgrade: 28 damage.
/// </summary>
public sealed class GoldenBullet : HermitCard
{
    private const int DamageAmount = 18;
    private const int UpgradedDamageAmount = 24;

    private int _currentCost = 3;
    [SavedProperty]
    public int CurrentCost
    {
        get
        {
            return _currentCost;
        }
        set
        {
            AssertMutable();
            _currentCost = value;
            EnergyCost.SetCustomBaseCost(_currentCost);
        }
    }
    
    public GoldenBullet() : base(3, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(DamageAmount, ValueProp.Move)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.Static(StaticHoverTip.Fatal)];

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        HermitSfx.PlayGun1();

        bool shouldTriggerFatal = play.Target!.Powers.All(p => p.ShouldOwnerDeathTriggerFatal());
        var attackCommand = await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target).WithHermitGunHitFx().Execute(ctx);

        if (shouldTriggerFatal && attackCommand.Results.Any(r => r.WasTargetKilled))
        {
            BuffFromPlay();
            // Sync to the deck version so the reduction persists after combat
            (DeckVersion as GoldenBullet)?.BuffFromPlay();
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UpgradedDamageAmount - DamageAmount);
    }
    protected override void AfterDowngraded()
    {
        UpdateCost();
    }
    private void BuffFromPlay()
    {
        CurrentCost = Math.Max(0, CurrentCost - 1);
        UpdateCost();
    }
    private void UpdateCost()
    {
        EnergyCost.SetCustomBaseCost(CurrentCost);
    }
}
