using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using HermitMod.Character;
using HermitMod.Extensions;
using HermitMod.Powers;
using HermitMod.Relics;
using HermitMod.Utility;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace HermitMod.Cards;

[Pool(typeof(HermitCardPool))]
public abstract class HermitCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    private bool? deadOnForCurrentPlay = null;

    /// <summary>
    /// Whether this card has a Dead On effect. Override to true in cards with Dead On mechanics.
    /// Used to show gold glow when the card is in the middle of the hand.
    /// </summary>
    public virtual bool HasDeadOn => false;

    /// <summary>
    /// Additional hover tips for this card (card previews, power tooltips, etc.).
    /// Override in subclasses to add tooltips for generated cards, referenced powers, etc.
    /// </summary>
    protected virtual IEnumerable<IHoverTip> AdditionalHoverTips => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            if (HasDeadOn)
                yield return HoverTipFactory.FromKeyword(HermitKeywords.DeadOn);
            foreach (var tip in AdditionalHoverTips)
                yield return tip;
        }
    }

    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    public bool IsDeadOn {
        get {
            if (!HasDeadOn)
                return false;

            return deadOnForCurrentPlay ?? IsDeadOnInCurrentHandState();
        }
    }

    private bool IsDeadOnInCurrentHandState()
    {
        if(((Type == CardType.Attack || Type == CardType.Skill) && Owner.Creature.HasPower<ConcentrationPower>()) || Owner.Creature.HasPower<CheatPower>())
            return true;

        var handCards = PileType.Hand.GetPile(Owner).Cards.ToList();
        int cardIndex = handCards.IndexOf(this);
        if (cardIndex == -1)
            return false;

        int handSize = handCards.Count;
        if (handSize % 2 == 0)
            return cardIndex == handSize / 2 - 1 || cardIndex == handSize / 2;
        else
            return cardIndex == handSize / 2;
    }

    internal void CaptureDeadOnForPlay()
    {
        deadOnForCurrentPlay = HasDeadOn && IsDeadOnInCurrentHandState();
    }
    internal void ForceSetDeadOnForPlay(bool value)
    {
        if (!HasDeadOn)
            return;
        deadOnForCurrentPlay = value;
    }

    internal void ClearDeadOnForPlay()
    {
        deadOnForCurrentPlay = null;
    }

    protected override bool ShouldGlowGoldInternal => HasDeadOn && IsDeadOn;

    protected async Task MaintanceDeadOnAfterPlayed(PlayerChoiceContext ctx, CardPlay? play)
    {
        DeadOnCounter.SetIsLastCardDeadOn(Owner, IsDeadOn);
        if (!IsDeadOn)
            return;
        DeadOnCounter.IncreaseCounter(Owner);

        var hermitPowers = Owner.Creature.Powers.OfType<HermitPower>().ToList();
        foreach (var hermitPower in hermitPowers)
            await hermitPower.AfterDeadOnTriggered(ctx, play);

        var hermitRelics = Owner.Relics.OfType<HermitRelic>().ToList();
        foreach (var hermitRelic in hermitRelics)
            await hermitRelic.AfterDeadOnTriggered(ctx, play);

    }


    protected sealed override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        bool IsSniped = Owner.Creature.HasPower<SnipePower>() && (Type == CardType.Attack || Type == CardType.Skill);
        if (HasDeadOn && IsDeadOn) {
            if (IsSniped)
                await BeforePlayInternalIfDeadOn(ctx, play);
            await BeforePlayInternalIfDeadOn(ctx, play);
        }
        await OnPlayInternal(ctx, play);
        if (HasDeadOn && IsDeadOn) {
            if (IsSniped)
                await AfterPlayInternalIfDeadOn(ctx, play);
            await AfterPlayInternalIfDeadOn(ctx, play);
        }
        await MaintanceDeadOnAfterPlayed(ctx, play);
        ClearDeadOnForPlay();
    }

    protected virtual Task BeforePlayInternalIfDeadOn(PlayerChoiceContext ctx, CardPlay play) => Task.CompletedTask;
    protected virtual Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play) => Task.CompletedTask;
    protected virtual Task AfterPlayInternalIfDeadOn(PlayerChoiceContext ctx, CardPlay play) => Task.CompletedTask;
}
