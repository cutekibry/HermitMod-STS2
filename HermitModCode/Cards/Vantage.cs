using HermitMod.Cards;
using HermitMod.Patches;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Cards;

/// <summary>
/// Gain 7 Block. Dead On: Draw and Upgrade a card.
/// Upgrade: Draw and Upgrade 2.
/// </summary>
public sealed class Vantage : HermitCard
{
    public override bool HasDeadOn => true;

    private const int BlockAmount = 7;
    private const int DrawUpgradeCount = 1;
    private const int UpgradedDrawUpgradeCount = 2;

    public Vantage() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.None) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar((decimal)BlockAmount, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);

        if (DeadOnHelper.IsDeadOn)
        {
            DeadOnHelper.IncrementDeadOnCount();
            int count = IsUpgraded ? UpgradedDrawUpgradeCount : DrawUpgradeCount;
            var drawn = await CardPileCmd.Draw(ctx, count, Owner, false);
            foreach (var card in drawn)
            {
                if (card != null && card.IsUpgradable)
                    CardCmd.Upgrade(card);
            }
        }
    }

    protected override void OnUpgrade()
    {
        // Upgrade changes draw count from 1 to 2 (reflected in upgradeDescription)
    }
}
