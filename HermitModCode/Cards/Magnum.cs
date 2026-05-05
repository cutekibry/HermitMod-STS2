using HermitMod.Cards;
using HermitMod.Utility;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// Discard 6 cards. For each card discarded, deal 8 damage.
/// Upgrade: 12 damage per card.
/// </summary>
public sealed class Magnum : HermitCard
{
    private const int DamageAmount = 6;
    private const int UpgradedDamageAmount = 8;
    private const int DiscardCount = 6;

    public Magnum() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar((decimal)DamageAmount, ValueProp.Move)];

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        // Prompt player to discard up to 6 cards
        var handCount = PileType.Hand.GetPile(Owner).Cards.Count;
        int maxDiscard = Math.Min(DiscardCount, handCount);

        if (maxDiscard > 0)
        {
            var selected = (await CardSelectCmd.FromHandForDiscard(
                ctx,
                Owner,
                new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, maxDiscard, maxDiscard),
                null,
                this
            )).ToList();

            if (selected.Count > 0)
            {
                await CardCmd.Discard(ctx, selected);

                await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
                HermitSfx.PlayGun1();

                // Deal damage once per card discarded
                for (int i = 0; i < selected.Count; i++)
                {
                    if (play.Target?.IsDead == true) break;
                    await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target).WithHermitGunHitFx().Execute(ctx);
                }
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UpgradedDamageAmount - DamageAmount);
    }
}
