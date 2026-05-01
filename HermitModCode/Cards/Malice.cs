using HermitMod.Utility;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// Exhaust a card. Deal 9 damage. If you Exhaust a Curse, deal damage to ALL enemies instead.
/// Upgrade: 12 damage.
/// </summary>
public sealed class Malice : HermitCard
{
    private const int DamageAmount = 9;
    private const int UpgradedDamageAmount = 12;

    public Malice() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(DamageAmount, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromKeyword(CardKeyword.Exhaust)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        // Prompt the player to exhaust a card from hand
        var hand = PileType.Hand.GetPile(Owner);
        if (hand == null || hand.Cards.Count == 0) return;

        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1);
        var selected = (await CardSelectCmd.FromHand(ctx, Owner, prefs, null, this)).FirstOrDefault();
        if (selected == null) return;

        bool exhaustedCurse = selected.Type == CardType.Curse;
        await CardCmd.Exhaust(ctx, selected);

        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        HermitSfx.PlayGun1();

        if (exhaustedCurse)
        {
            // Exhausted a Curse — deal damage to ALL enemies
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .TargetingAllOpponents(CombatState!)
                .WithHermitFireHitFx()
                .Execute(ctx);
        }
        else
        {
            // Normal — deal damage to the single target
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Targeting(play.Target!)
                .WithHermitGunHitFx()
                .Execute(ctx);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UpgradedDamageAmount - DamageAmount);
    }
}
