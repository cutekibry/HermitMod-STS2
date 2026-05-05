using HermitMod.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Relics;

/// <summary>
/// Whenever you trigger a Dead On effect, deal 2 damage to ALL enemies.
/// </summary>
public sealed class BlackPowder : HermitRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    private const int Damage = 2;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(Damage, ValueProp.Unpowered)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(HermitKeywords.DeadOn)
    ];

    public override async Task AfterDeadOnTriggered(PlayerChoiceContext ctx, CardPlay? play)
    {
        await CreatureCmd.Damage(ctx, Owner.Creature.CombatState!.HittableEnemies, DynamicVars.Damage, Owner.Creature);
    }


}
