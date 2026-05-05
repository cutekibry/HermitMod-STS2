using BaseLib.Abstracts;
using HermitMod.Cards;
using HermitMod.Extensions;
using HermitMod.Relics;
using HermitMod.Utility;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Character;

public class Hermit : PlaceholderCharacterModel
{
    public const string CharacterId = "HermitMod";

    public static readonly Color Color = new("9e6a34");
    public static readonly Color CardBackColor = new("edd4b8");

    public override Color NameColor => Color;
    public override Color MapDrawingColor => Color;
    public override Color RemoteTargetingLineColor => new("9e6a34ff");
    public override Color RemoteTargetingLineOutline => new("5a3d20ff");
    public override CharacterGender Gender => CharacterGender.Neutral;
    public override int StartingHp => 70;
    public override int StartingGold => 99;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<StrikeHermit>(),
        ModelDb.Card<StrikeHermit>(),
        ModelDb.Card<StrikeHermit>(),
        ModelDb.Card<StrikeHermit>(),
        ModelDb.Card<DefendHermit>(),
        ModelDb.Card<DefendHermit>(),
        ModelDb.Card<DefendHermit>(),
        ModelDb.Card<DefendHermit>(),
        ModelDb.Card<Covet>(),
        ModelDb.Card<Snapshot>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<OldLocket>()
    ];

    public override CardPoolModel CardPool => ModelDb.CardPool<HermitCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<HermitRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<HermitPotionPool>();

    // Character select background
    public override string CustomCharacterSelectBg => "res://HermitMod/scenes/hermit_select_bg.tscn";

    // Character select button icon
    public override string CustomCharacterSelectIconPath => "HermitButton.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "HermitButton.png".CharacterUiPath();

    // In-game UI icon
    public override Control? CustomIcon
    {
        get
        {
            var texture = ResourceLoader.Load<Texture2D>("res://HermitMod/images/charui/character_icon_hermit.png");
            if (texture == null) return null;
            var container = new Control();
            container.SetAnchorsPreset(Control.LayoutPreset.FullRect);
            var rect = new TextureRect();
            rect.Texture = texture;
            rect.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
            rect.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
            rect.SetAnchorsPreset(Control.LayoutPreset.FullRect);
            rect.GrowHorizontal = Control.GrowDirection.Both;
            rect.GrowVertical = Control.GrowDirection.Both;
            container.AddChild(rect);
            return container;
        }
    }
    public override string CustomIconTexturePath => "character_icon_hermit.png".CharacterUiPath();
    public override string CustomIconOutlineTexturePath => "character_icon_hermit_outline.png".CharacterUiPath();

    // Map marker
    public override string CustomMapMarkerPath => "map_marker_hermit.png".CharacterUiPath();

    // Multiplayer arm textures (relic selection / rock-paper-scissors)
    public override string CustomArmPointingTexturePath => "hermit_arm_point.png".CharacterUiPath();
    public override string CustomArmRockTexturePath => "hermit_arm_rock.png".CharacterUiPath();
    public override string CustomArmPaperTexturePath => "hermit_arm_paper.png".CharacterUiPath();
    public override string CustomArmScissorsTexturePath => "hermit_arm_scissors.png".CharacterUiPath();

    // Sound effects
    public override string CharacterSelectSfx
    {
        get
        {
            ModAudio.PlayGlobalSfx("hermit_select", 5f);
            return "";
        }
    }
    public override string CharacterTransitionSfx => "event:/sfx/ui/wipe_ironclad";
    public override string CustomAttackSfx => "event:/sfx/enemy/enemy_attacks/crossbow_ruby_raider/crossbow_ruby_raider_reload";
    // public override string? CustomCastSfx => null;

    // Pure Godot creature visual packaged with this mod.
    public override string CustomVisualPath => "res://HermitMod/scenes/hermit_creature_visual.tscn";

    // Leave these on the base-game defaults unless we have packaged scenes for them.
    public override string CustomEnergyCounterPath => base.CustomEnergyCounterPath;
    public override string CustomMerchantAnimPath => base.CustomMerchantAnimPath;
}
