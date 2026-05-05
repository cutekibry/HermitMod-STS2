using HermitMod.Cards;
using HermitMod.Utility;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Powers;

/// <summary>
/// Put the first X Dead On cards triggered each turn back into your hand.
/// </summary>
public sealed class ComboPower : HermitPower
{
    private class Data
    {
        public int deadOnCardsPlayed = 0;
    }

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Math.Max(0, Amount - GetInternalData<Data>().deadOnCardsPlayed);

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override (PileType, CardPilePosition) ModifyCardPlayResultPileTypeAndPosition(CardModel card, bool isAutoPlay, ResourceInfo resources, PileType pileType, CardPilePosition position)
    {
        if (
            GetInternalData<Data>().deadOnCardsPlayed >= Amount
        || card.Owner.Creature != Owner
        || (card.Type != CardType.Attack && card.Type != CardType.Skill)
        || card is not HermitCard { IsDeadOn: true }
        )
            return (pileType, position);

        Flash();
        SetDeadOnCardsPlayed(GetInternalData<Data>().deadOnCardsPlayed + 1);
        return (PileType.Hand, CardPilePosition.Bottom);
    }

    public override Task AfterSideTurnStart(CombatSide side, ICombatState combatState)
    {
        if (side != Owner.Side)
        {
            return Task.CompletedTask;
        }

        SetDeadOnCardsPlayed(0);
        return Task.CompletedTask;
    }

    private void SetDeadOnCardsPlayed(int value)
    {
        GetInternalData<Data>().deadOnCardsPlayed = value;
        InvokeDisplayAmountChanged();
    }
}
