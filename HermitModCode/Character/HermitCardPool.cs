using System.Collections.Generic;
using System.Linq;
using BaseLib.Abstracts;
using HermitMod.Epochs;
using HermitMod.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Unlocks;

namespace HermitMod.Character;

public class HermitCardPool : CustomCardPoolModel
{
    public override string Title => Hermit.CharacterId;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();

    // Warm brown/gold card frame (matching original Hermit STS1 backgrounds)
    public override float H => 0.088f;
    public override float S => 0.48f;
    public override float V => 0.47f;

    public override Color DeckEntryCardColor => new("B1814C");

    public override bool IsColorless => false;

    protected override IEnumerable<CardModel> FilterThroughEpochs(UnlockState unlockState, IEnumerable<CardModel> cards)
    {
        // Exclude Basic/Starter cards from the reward pool (Strike, Defend, Snapshot, Memento, Covet)
        var list = cards.Where(c => c.Rarity != CardRarity.Basic).ToList();

        if (!unlockState.IsEpochRevealed<Hermit2Epoch>())
        {
            list.RemoveAll(c => Hermit2Epoch.Cards.Any(card => card.Id == c.Id));
        }

        if (!unlockState.IsEpochRevealed<Hermit4Epoch>())
        {
            list.RemoveAll(c => Hermit4Epoch.Cards.Any(card => card.Id == c.Id));
        }

        if (!unlockState.IsEpochRevealed<Hermit5Epoch>())
        {
            list.RemoveAll(c => Hermit5Epoch.Cards.Any(card => card.Id == c.Id));
        }

        if (!unlockState.IsEpochRevealed<Hermit7Epoch>())
        {
            list.RemoveAll(c => Hermit7Epoch.Cards.Any(card => card.Id == c.Id));
        }

        return list;
    }
}
