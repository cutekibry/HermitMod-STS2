using HermitMod.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// Add an upgraded Defend to your hand. It costs 0 this turn.
/// Upgrade: Add 2 Defends.
/// </summary>
public sealed class TakeCover : HermitCard
{
    private const int DefendCount = 1;
    private const int UpgradedDefendCount = 2;

    public TakeCover() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.None) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(DefendCount)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        int count = DynamicVars.Cards.IntValue;
        for (int i = 0; i < count; i++)
        {
            var defend = CombatState.CreateCard<Defend_Hermit>(Owner);
            defend.EnergyCost.SetUntilPlayed(0, false);
            await CardPileCmd.AddGeneratedCardToCombat(defend, PileType.Hand, addedByPlayer: true);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(UpgradedDefendCount - DefendCount);
    }
}
