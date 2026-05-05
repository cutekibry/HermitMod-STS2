using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace HermitMod.Relics;

/// <summary>
/// Whenever you draw a Curse, your next attack deals 3 more damage.
/// </summary>
public sealed class CharredGlove : HermitRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;

    private const int VigorAmount = 3;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<VigorPower>(VigorAmount)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<VigorPower>()];

    public override async Task AfterCardDrawn(PlayerChoiceContext ctx, CardModel card, bool fromHandDraw)
    {
        if (card.Owner?.Creature != Owner?.Creature) return;
        if (card.Type == CardType.Curse)
        {
            Flash();
            await PowerCmd.Apply<VigorPower>(ctx, Owner!.Creature, DynamicVars["VigorPower"].BaseValue, Owner.Creature, null);
        }
    }
}
