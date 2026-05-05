using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Relics;

/// <summary>
/// Whenever you remove or Transform a card from your deck, heal 15 HP.
/// </summary>
public sealed class StraightRazor : HermitRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    private const int HealAmount = 15;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new HealVar(HealAmount)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Transform)];

    public override async Task BeforeCardRemoved(CardModel card)
    {
        await Heal();
    }

    public async Task Heal()
    {
        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
    }
}
