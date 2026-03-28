using HermitMod.Cards;
using HermitMod.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Cards;

/// <summary>
/// Gain 2 Rugged. Increase the cost of this card by 1 this combat.
/// Upgrade: Gain 3 Rugged.
/// </summary>
public sealed class HeroicBravado : HermitCard
{
    private const int RuggedAmt = 2;
    private const int UpgradedRuggedAmt = 3;

    public HeroicBravado() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.None) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<RuggedPower>((decimal)RuggedAmt)];

    private int CurrentRugged => IsUpgraded ? UpgradedRuggedAmt : RuggedAmt;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<RuggedPower>(Owner.Creature, CurrentRugged, Owner.Creature, this);

        // Increase cost by 1 for the rest of combat
        EnergyCost.SetCustomBaseCost(EnergyCost.GetWithModifiers(default) + 1);
    }

    protected override void OnUpgrade() { }
}
