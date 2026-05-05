using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace HermitMod.Cards;

/// <summary>
/// Apply 1 Vulnerable to EVERYONE (all enemies AND player). Retain.
/// </summary>
[Pool(typeof(CurseCardPool))]
public sealed class MementoCard : HermitCard
{
    public override int MaxUpgradeLevel => 0;
    private const int VulnAmount = 1;

    public MementoCard() : base(0, CardType.Curse, CardRarity.Curse, TargetType.None) { }
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<VulnerablePower>(VulnAmount)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<VulnerablePower>()];
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        foreach (var enemy in CombatState!.HittableEnemies)
        {
            await PowerCmd.Apply<VulnerablePower>(ctx, enemy, DynamicVars["VulnerablePower"].BaseValue, Owner.Creature, this);
        }

        await PowerCmd.Apply<VulnerablePower>(ctx, Owner.Creature, DynamicVars["VulnerablePower"].BaseValue, Owner.Creature, this);
    }
}
