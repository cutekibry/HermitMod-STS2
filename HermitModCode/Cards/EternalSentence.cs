using HermitMod.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace HermitMod.Cards;

/// <summary>
/// Ethereal. All your cards cost 0. You can no longer gain Energy. At the start of your turn, add a random Curse to your hand.
/// Upgrade: Remove Ethereal.
/// </summary>
public sealed class EternalSentence : HermitCard
{

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];



    public EternalSentence() : base(3, CardType.Power, CardRarity.Ancient, TargetType.Self) { }
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<EternalSentencePower>(ctx, Owner.Creature, 1, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Ethereal);
    }
}
