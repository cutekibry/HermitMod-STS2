using HermitMod.Cards;
using HermitMod.Utility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// Deal 16 damage. Discard your hand. Draw that many cards.
/// Upgrade: 20 damage.
/// </summary>
public sealed class Roulette : HermitCard
{
    private const int DamageAmount = 16;
    private const int UpgradedDamageAmount = 20;

    public Roulette() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar((decimal)DamageAmount, ValueProp.Move)];

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        HermitSfx.PlaySpin();
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        HermitSfx.PlayGun2();
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target).WithHermitGunHitFx().Execute(ctx);

        // Count cards in hand, discard them all, then draw that many
        var hand = PileType.Hand.GetPile(Owner);
        int handSize = hand?.Cards.Count ?? 0;

        if (handSize > 0)
        {
            var cardsToDiscard = hand!.Cards.ToList();
            foreach (var card in cardsToDiscard)
            {
                await CardCmd.Discard(ctx, card);
            }
            await CardPileCmd.Draw(ctx, handSize, Owner, false);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UpgradedDamageAmount - DamageAmount);
    }
}
