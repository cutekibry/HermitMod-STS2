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
/// Apply 3 Bruise to ALL enemies. Bruise does not wear off this turn.
/// Upgrade: Apply 4 Bruise.
/// </summary>
public sealed class Horror : HermitCard
{
    private const int BruiseAmount = 3;
    private const int UpgradedBruiseAmount = 5;

    public Horror() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<BruisePower>((decimal)BruiseAmount)];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<BruisePower>()];

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        int amount = IsUpgraded ? UpgradedBruiseAmount : BruiseAmount;
        foreach (var enemy in CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<BruisePower>(ctx, enemy, amount, Owner.Creature, this);
            // Apply Horror to each enemy so their Bruise doesn't wear off this turn
            await PowerCmd.Apply<HorrorPower>(ctx, enemy, 1, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["BruisePower"].UpgradeValueBy(UpgradedBruiseAmount - BruiseAmount);
    }
}
