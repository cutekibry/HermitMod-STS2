using BaseLib.Abstracts;
using BaseLib.Utils;
using HermitMod.Character;
using HermitMod.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace HermitMod.Cards;

/// <summary>
/// Apply 1 Vulnerable to EVERYONE (all enemies AND player). Retain.
/// Upgrade: Apply 2 Vulnerable.
/// </summary>
[Pool(typeof(HermitCardPool))]
public sealed class MementoCard : CustomCardModel
{
    private const int VulnAmount = 1;
    private const int UpgradedVulnAmount = 2;

    public MementoCard() : base(0, CardType.Skill, CardRarity.Basic, TargetType.AllEnemies) { }
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

    public override string CustomPortraitPath => "memento_card.png".BigCardImagePath();
    public override string PortraitPath => "memento_card.png".CardImagePath();
    public override string BetaPortraitPath => "beta/memento_card.png".CardImagePath();

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<VulnerablePower>((decimal)VulnAmount)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<VulnerablePower>()];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        int amount = IsUpgraded ? UpgradedVulnAmount : VulnAmount;

        // Apply to ALL enemies
        foreach (var enemy in CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<VulnerablePower>(ctx, enemy, amount, Owner.Creature, this);
        }

        // Apply to player too (EVERYONE)
        await PowerCmd.Apply<VulnerablePower>(ctx, Owner.Creature, amount, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["VulnerablePower"].UpgradeValueBy(UpgradedVulnAmount - VulnAmount);
    }
}
