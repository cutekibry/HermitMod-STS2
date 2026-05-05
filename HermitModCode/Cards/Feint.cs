using HermitMod.Cards;
using HermitMod.Character;
using HermitMod.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.HoverTips;

namespace HermitMod.Cards;

public class Feint() : HermitCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    private const int BlockAmount = 3;
    private const int UpgradedBlockAmount = 5;
    private const int BruiseAmount = 2;
    private const int UpgradedBruiseAmount = 3;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(BlockAmount, ValueProp.Move), new PowerVar<BruisePower>(BruiseAmount)];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<BruisePower>()];


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);

        // Apply Bruise to ALL enemies
        int bruise = DynamicVars["BruisePower"].IntValue;
        foreach (var enemy in CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<BruisePower>(ctx, enemy, bruise, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(UpgradedBlockAmount - BlockAmount);
        DynamicVars["BruisePower"].UpgradeValueBy(UpgradedBruiseAmount - BruiseAmount);
    }
}
