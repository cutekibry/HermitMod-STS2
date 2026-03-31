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
/// Deal 10 damage. Lose 2 HP. Exhaust.
/// After playing, permanently increase ALL Cursed Weapons' damage by 1.
/// Upgrade: Cost becomes 0.
/// </summary>
public sealed class CursedWeapon : HermitCard
{
    private const int BaseDamageAmount = 10;
    private const int HpLossAmount = 2;
    private const int ScaleAmount = 1;

    /// <summary>
    /// Tracks the accumulated bonus damage from previous plays. Persists via DeckVersion.
    /// </summary>
    public int BonusDamage { get; set; }

    public CursedWeapon() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar((decimal)BaseDamageAmount, ValueProp.Move),
        new HpLossVar(HpLossAmount)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        // Lose HP first (matches original STS1 order)
        await CreatureCmd.Damage(ctx, Owner.Creature, DynamicVars.HpLoss.BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered, null, null);

        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);

        decimal totalDamage = DynamicVars.Damage.BaseValue + BonusDamage;
        await DamageCmd.Attack(totalDamage).FromCard(this).Targeting(play.Target).WithHermitFireHitFx().Execute(ctx);

        // After dealing damage, permanently increase ALL Cursed Weapons' bonus damage
        ScaleAllCursedWeapons();
    }

    private void ScaleAllCursedWeapons()
    {
        foreach (var pileType in new[] { PileType.Hand, PileType.Draw, PileType.Discard, PileType.Exhaust })
        {
            var pile = pileType.GetPile(Owner);
            if (pile == null) continue;
            foreach (var card in pile.Cards)
            {
                if (card is CursedWeapon cw)
                    cw.BonusDamage += ScaleAmount;
            }
        }

        // Scale this card too
        BonusDamage += ScaleAmount;

        // Scale deck versions so the increase persists across combats
        if (DeckVersion is CursedWeapon deckCw)
            deckCw.BonusDamage += ScaleAmount;

        foreach (var pileType in new[] { PileType.Hand, PileType.Draw, PileType.Discard, PileType.Exhaust })
        {
            var pile = pileType.GetPile(Owner);
            if (pile == null) continue;
            foreach (var card in pile.Cards)
            {
                if (card is CursedWeapon otherCw && otherCw != this)
                    if (otherCw.DeckVersion is CursedWeapon otherDeckCw)
                        otherDeckCw.BonusDamage += ScaleAmount;
            }
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
