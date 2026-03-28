using BaseLib.Abstracts;
using HermitMod.Cards;
using HermitMod.Extensions;
using HermitMod.Relics;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace HermitMod.Character;

public class Hermit : PlaceholderCharacterModel
{
    public const string CharacterId = "HermitMod";

    public static readonly Color Color = new("6B3A6B"); // Dark purple/mauve

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Neutral;
    public override int StartingHp => 70;
    public override int StartingGold => 99;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<Strike_Hermit>(),
        ModelDb.Card<Strike_Hermit>(),
        ModelDb.Card<Strike_Hermit>(),
        ModelDb.Card<Strike_Hermit>(),
        ModelDb.Card<Strike_Hermit>(),
        ModelDb.Card<Defend_Hermit>(),
        ModelDb.Card<Defend_Hermit>(),
        ModelDb.Card<Defend_Hermit>(),
        ModelDb.Card<Defend_Hermit>(),
        ModelDb.Card<Snapshot>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<OldLocket>()
    ];

    public override CardPoolModel CardPool => ModelDb.CardPool<HermitCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<HermitRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<HermitPotionPool>();

    // Character select background (full portrait)
    public override string CustomCharacterSelectBg => "res://HermitMod/scenes/hermit_select_bg.tscn";

    // Character select button icon
    public override string CustomCharacterSelectIconPath => "HermitButton.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "HermitButton.png".CharacterUiPath();

    // In-game UI icons
    public override string CustomIconTexturePath => "HermitButton.png".CharacterUiPath();

    // Map marker
    public override string CustomMapMarkerPath => "HermitButton.png".CharacterUiPath();

    // Sound effects — use crossbow sound as gun placeholder
    public override string CharacterTransitionSfx => "event:/sfx/ui/wipe_ironclad";
    public override string? CustomAttackSfx => "event:/sfx/enemy/enemy_attacks/crossbow_ruby_raider/crossbow_ruby_raider_reload";
    public override string? CustomCastSfx => null;

    // Build the combat visual programmatically with animated body parts
    public override NCreatureVisuals? CreateCustomVisuals()
    {
        return HermitVisualBuilder.Build();
    }
}
