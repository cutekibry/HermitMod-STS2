using Godot;
using MegaCrit.Sts2.Core.Nodes.Combat;
using BaseLib.Utils.NodeFactories;

namespace HermitMod.Character;

/// <summary>
/// Builds the Hermit's combat visual from individual body part sprites extracted
/// from the original STS1 Spine atlas, with AnimationPlayer-driven animations.
/// </summary>
public static class HermitVisualBuilder
{
    private const string CharDir = "res://HermitMod/images/char/";

    // Base waist Y position — places feet near y=0 (ground level)
    private const float WaistY = -48f;

    public static NCreatureVisuals Build()
    {
        var root = new NCreatureVisuals();

        // Bounds (interaction area)
        var bounds = new Control();
        bounds.Name = "Bounds";
        bounds.UniqueNameInOwner = true;
        bounds.Position = new Vector2(-130, -260);
        bounds.Size = new Vector2(260, 260);
        root.AddChild(bounds);
        bounds.Owner = root;

        // Required marker nodes for VFX targeting (block gain, damage, etc.)
        var centerPos = new Marker2D();
        centerPos.Name = "CenterPos";
        centerPos.UniqueNameInOwner = true;
        centerPos.Position = new Vector2(0, -120);
        root.AddChild(centerPos);
        centerPos.Owner = root;

        var orbPos = new Marker2D();
        orbPos.Name = "OrbPos";
        orbPos.UniqueNameInOwner = true;
        orbPos.Position = new Vector2(-130, -120);
        root.AddChild(orbPos);
        orbPos.Owner = root;

        var talkPos = new Marker2D();
        talkPos.Name = "TalkPos";
        talkPos.UniqueNameInOwner = true;
        talkPos.Position = new Vector2(60, -310);
        root.AddChild(talkPos);
        talkPos.Owner = root;

        var intentPos = new Marker2D();
        intentPos.Name = "IntentPos";
        intentPos.UniqueNameInOwner = true;
        intentPos.Position = new Vector2(0, -270);
        root.AddChild(intentPos);
        intentPos.Owner = root;

        // Visuals container
        var visuals = new Node2D();
        visuals.Name = "Visuals";
        visuals.UniqueNameInOwner = true;
        visuals.Scale = new Vector2(1.3f, 1.3f);
        root.AddChild(visuals);
        visuals.Owner = root;

        // Shadow at ground level
        var shadow = CreateSprite("Shadow", CharDir + "shadow.png", new Vector2(0, -5));
        visuals.AddChild(shadow);
        shadow.Owner = root;

        // Waist (main body pivot - everything attaches here)
        var waist = new Node2D();
        waist.Name = "Waist";
        waist.Position = new Vector2(2, WaistY);
        visuals.AddChild(waist);
        waist.Owner = root;

        // Legs (feet should land near shadow/ground)
        var rightLeg = CreateSprite("RightLeg", CharDir + "leg_right.png", new Vector2(18, 43));
        waist.AddChild(rightLeg);
        rightLeg.Owner = root;

        var leftLeg = CreateSprite("LeftLeg", CharDir + "leg_left.png", new Vector2(-20, 45));
        waist.AddChild(leftLeg);
        leftLeg.Owner = root;

        // Body
        var body = CreateSprite("Body", CharDir + "body.png", new Vector2(0, -5));
        body.Offset = new Vector2(0, -50);
        waist.AddChild(body);
        body.Owner = root;

        // Right arm (behind body, mostly hidden)
        var rightArm = CreateSprite("RightArm", CharDir + "right_hand.png", new Vector2(52, -69));
        rightArm.Rotation = 0.20f;
        waist.AddChild(rightArm);
        rightArm.Owner = root;

        // Left arm (pivot for gun — positioned at left sleeve opening)
        var leftArm = new Node2D();
        leftArm.Name = "LeftArm";
        leftArm.Position = new Vector2(-32, -60);
        leftArm.Rotation = -0.15f;
        waist.AddChild(leftArm);
        leftArm.Owner = root;

        var handGun = CreateSprite("HandGun", CharDir + "hand_gun.png", new Vector2(-5, 5));
        leftArm.AddChild(handGun);
        handGun.Owner = root;

        var gun = CreateSprite("Gun", CharDir + "gun.png", new Vector2(-12, -12));
        gun.Rotation = 0.3f;
        gun.Scale = new Vector2(0.85f, 0.85f);
        leftArm.AddChild(gun);
        gun.Owner = root;

        // Head — hat sits on top, eyes peek out from under the brim
        var head = new Node2D();
        head.Name = "Head";
        head.Position = new Vector2(-2, -105);
        waist.AddChild(head);
        head.Owner = root;

        var hat = CreateSprite("Hat", CharDir + "hat.png", new Vector2(20, -45));
        head.AddChild(hat);
        hat.Owner = root;

        var eye = CreateSprite("Eye", CharDir + "eye.png", new Vector2(18, -12));
        head.AddChild(eye);
        eye.Owner = root;

        // Corpse sprite (hidden, shown on death)
        var corpse = CreateSprite("Corpse", CharDir + "corpse.png", new Vector2(0, -20));
        corpse.Visible = false;
        corpse.Offset = new Vector2(0, -30);
        visuals.AddChild(corpse);
        corpse.Owner = root;

        // AnimationPlayer with all combat animations
        var animPlayer = CreateAnimationPlayer(waist, head, leftArm, gun, rightArm, visuals, corpse);
        root.AddChild(animPlayer);
        animPlayer.Owner = root;

        // Return to idle after any one-shot animation finishes
        animPlayer.AnimationFinished += (Godot.StringName animName) =>
        {
            if (animName != "idle" && animName != "die")
            {
                animPlayer.Play("idle");
            }
        };

        // Auto-start idle
        animPlayer.CallDeferred("play", "idle");

        return root;
    }

    private static Sprite2D CreateSprite(string name, string texturePath, Vector2 position)
    {
        var sprite = new Sprite2D();
        sprite.Name = name;
        sprite.Position = position;
        sprite.Texture = GD.Load<Texture2D>(texturePath);
        return sprite;
    }

    private static AnimationPlayer CreateAnimationPlayer(
        Node2D waist, Node2D head, Node2D leftArm, Node2D gun,
        Sprite2D rightArm, Node2D visuals, Sprite2D corpse)
    {
        var player = new AnimationPlayer();
        player.Name = "AnimPlayer";

        var lib = new AnimationLibrary();
        lib.AddAnimation("idle", CreateIdleAnimation());
        lib.AddAnimation("hurt", CreateHitAnimation());
        lib.AddAnimation("attack", CreateAttackAnimation());
        lib.AddAnimation("cast", CreateCastAnimation());
        lib.AddAnimation("die", CreateDeadAnimation());

        player.AddAnimationLibrary("", lib);
        return player;
    }

    private static Animation CreateIdleAnimation()
    {
        var anim = new Animation();
        anim.Length = 1.6666f;
        anim.LoopMode = Animation.LoopModeEnum.Linear;

        // Waist gentle sway
        int t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, "Visuals/Waist:position");
        anim.TrackSetInterpolationType(t, Animation.InterpolationType.Cubic);
        anim.TrackInsertKey(t, 0f, new Vector2(2, WaistY));
        anim.TrackInsertKey(t, 0.8333f, new Vector2(6.84f, WaistY));
        anim.TrackInsertKey(t, 1.6666f, new Vector2(2, WaistY));

        // Head bob
        t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, "Visuals/Waist/Head:position");
        anim.TrackSetInterpolationType(t, Animation.InterpolationType.Cubic);
        anim.TrackInsertKey(t, 0f, new Vector2(-2, -105));
        anim.TrackInsertKey(t, 0.8333f, new Vector2(-2, -103.2f));
        anim.TrackInsertKey(t, 1.6666f, new Vector2(-2, -105));

        // Head tilt
        t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, "Visuals/Waist/Head:rotation");
        anim.TrackSetInterpolationType(t, Animation.InterpolationType.Cubic);
        anim.TrackInsertKey(t, 0f, 0f);
        anim.TrackInsertKey(t, 0.8333f, 0.111f);
        anim.TrackInsertKey(t, 1.6666f, 0f);

        // Left arm sway
        t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, "Visuals/Waist/LeftArm:position");
        anim.TrackSetInterpolationType(t, Animation.InterpolationType.Cubic);
        anim.TrackInsertKey(t, 0f, new Vector2(-32, -60));
        anim.TrackInsertKey(t, 0.8333f, new Vector2(-36, -66));
        anim.TrackInsertKey(t, 1.6666f, new Vector2(-32, -60));

        // Left arm rotation
        t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, "Visuals/Waist/LeftArm:rotation");
        anim.TrackSetInterpolationType(t, Animation.InterpolationType.Cubic);
        anim.TrackInsertKey(t, 0f, -0.15f);
        anim.TrackInsertKey(t, 0.8333f, -0.35f);
        anim.TrackInsertKey(t, 1.6666f, -0.15f);

        // Right arm rotation
        t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, "Visuals/Waist/RightArm:rotation");
        anim.TrackSetInterpolationType(t, Animation.InterpolationType.Cubic);
        anim.TrackInsertKey(t, 0f, 0.20f);
        anim.TrackInsertKey(t, 0.8333f, 0.45f);
        anim.TrackInsertKey(t, 1.6666f, 0.20f);

        // Gun wobble
        t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, "Visuals/Waist/LeftArm/Gun:rotation");
        anim.TrackSetInterpolationType(t, Animation.InterpolationType.Cubic);
        anim.TrackInsertKey(t, 0f, 0.4f);
        anim.TrackInsertKey(t, 1.1667f, 0.154f);
        anim.TrackInsertKey(t, 1.6667f, 0.4f);

        return anim;
    }

    private static Animation CreateHitAnimation()
    {
        var anim = new Animation();
        anim.Length = 0.5f;
        anim.LoopMode = Animation.LoopModeEnum.None;

        // Waist recoil back
        int t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, "Visuals/Waist:position");
        anim.TrackSetInterpolationType(t, Animation.InterpolationType.Cubic);
        anim.TrackInsertKey(t, 0f, new Vector2(2, WaistY));
        anim.TrackInsertKey(t, 0.0833f, new Vector2(-8.7f, WaistY + 1.33f));
        anim.TrackInsertKey(t, 0.5f, new Vector2(2, WaistY));

        // Waist rotation
        t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, "Visuals/Waist:rotation");
        anim.TrackSetInterpolationType(t, Animation.InterpolationType.Cubic);
        anim.TrackInsertKey(t, 0f, 0f);
        anim.TrackInsertKey(t, 0.0833f, -0.32f);
        anim.TrackInsertKey(t, 0.5f, 0f);

        // Head whip back
        t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, "Visuals/Waist/Head:position");
        anim.TrackSetInterpolationType(t, Animation.InterpolationType.Cubic);
        anim.TrackInsertKey(t, 0f, new Vector2(-2, -105));
        anim.TrackInsertKey(t, 0.0833f, new Vector2(9.55f, -110.88f)); // hit recoil head
        anim.TrackInsertKey(t, 0.5f, new Vector2(-2, -105));

        // Head rotation on hit
        t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, "Visuals/Waist/Head:rotation");
        anim.TrackSetInterpolationType(t, Animation.InterpolationType.Cubic);
        anim.TrackInsertKey(t, 0f, 0f);
        anim.TrackInsertKey(t, 0.0833f, 0.446f);
        anim.TrackInsertKey(t, 0.5f, 0f);

        // Right arm fling
        t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, "Visuals/Waist/RightArm:rotation");
        anim.TrackSetInterpolationType(t, Animation.InterpolationType.Cubic);
        anim.TrackInsertKey(t, 0f, 0.20f);
        anim.TrackInsertKey(t, 0.0833f, -0.28f);
        anim.TrackInsertKey(t, 0.5f, 0.20f);

        return anim;
    }

    private static Animation CreateAttackAnimation()
    {
        var anim = new Animation();
        anim.Length = 0.5f;
        anim.LoopMode = Animation.LoopModeEnum.None;

        // Lunge forward
        int t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, "Visuals/Waist:position");
        anim.TrackSetInterpolationType(t, Animation.InterpolationType.Cubic);
        anim.TrackInsertKey(t, 0f, new Vector2(2, WaistY));
        anim.TrackInsertKey(t, 0.1f, new Vector2(30, WaistY));
        anim.TrackInsertKey(t, 0.15f, new Vector2(40, WaistY));
        anim.TrackInsertKey(t, 0.5f, new Vector2(2, WaistY));

        // Gun arm swings forward on attack
        t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, "Visuals/Waist/LeftArm:rotation");
        anim.TrackSetInterpolationType(t, Animation.InterpolationType.Cubic);
        anim.TrackInsertKey(t, 0f, -0.15f);
        anim.TrackInsertKey(t, 0.1f, 0.3f);
        anim.TrackInsertKey(t, 0.15f, 0.4f);
        anim.TrackInsertKey(t, 0.5f, -0.15f);

        // Gun kicks back
        t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, "Visuals/Waist/LeftArm/Gun:rotation");
        anim.TrackSetInterpolationType(t, Animation.InterpolationType.Cubic);
        anim.TrackInsertKey(t, 0f, 0.4f);
        anim.TrackInsertKey(t, 0.12f, 0.1f);
        anim.TrackInsertKey(t, 0.18f, 0.8f); // recoil
        anim.TrackInsertKey(t, 0.5f, 0.4f);

        // Body leans into attack
        t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, "Visuals/Waist:rotation");
        anim.TrackSetInterpolationType(t, Animation.InterpolationType.Cubic);
        anim.TrackInsertKey(t, 0f, 0f);
        anim.TrackInsertKey(t, 0.1f, 0.15f);
        anim.TrackInsertKey(t, 0.5f, 0f);

        return anim;
    }

    private static Animation CreateCastAnimation()
    {
        var anim = new Animation();
        anim.Length = 0.6f;
        anim.LoopMode = Animation.LoopModeEnum.None;

        // Slight step back
        int t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, "Visuals/Waist:position");
        anim.TrackSetInterpolationType(t, Animation.InterpolationType.Cubic);
        anim.TrackInsertKey(t, 0f, new Vector2(2, WaistY));
        anim.TrackInsertKey(t, 0.15f, new Vector2(-5, WaistY - 5));
        anim.TrackInsertKey(t, 0.6f, new Vector2(2, WaistY));

        // Arms raise up for cast
        t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, "Visuals/Waist/LeftArm:rotation");
        anim.TrackSetInterpolationType(t, Animation.InterpolationType.Cubic);
        anim.TrackInsertKey(t, 0f, -0.15f);
        anim.TrackInsertKey(t, 0.15f, -0.6f);
        anim.TrackInsertKey(t, 0.6f, -0.15f);

        t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, "Visuals/Waist/LeftArm:position");
        anim.TrackSetInterpolationType(t, Animation.InterpolationType.Cubic);
        anim.TrackInsertKey(t, 0f, new Vector2(-32, -60));
        anim.TrackInsertKey(t, 0.15f, new Vector2(-32, -76));
        anim.TrackInsertKey(t, 0.6f, new Vector2(-32, -60));

        t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, "Visuals/Waist/RightArm:rotation");
        anim.TrackSetInterpolationType(t, Animation.InterpolationType.Cubic);
        anim.TrackInsertKey(t, 0f, 0.20f);
        anim.TrackInsertKey(t, 0.15f, -0.35f);
        anim.TrackInsertKey(t, 0.6f, 0.20f);

        // Head tilts up
        t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, "Visuals/Waist/Head:rotation");
        anim.TrackSetInterpolationType(t, Animation.InterpolationType.Cubic);
        anim.TrackInsertKey(t, 0f, 0f);
        anim.TrackInsertKey(t, 0.15f, -0.15f);
        anim.TrackInsertKey(t, 0.6f, 0f);

        return anim;
    }

    private static Animation CreateDeadAnimation()
    {
        var anim = new Animation();
        anim.Length = 0.8f;
        anim.LoopMode = Animation.LoopModeEnum.None;

        // Collapse - waist drops to ground
        int t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, "Visuals/Waist:position");
        anim.TrackSetInterpolationType(t, Animation.InterpolationType.Cubic);
        anim.TrackInsertKey(t, 0f, new Vector2(2, WaistY));
        anim.TrackInsertKey(t, 0.4f, new Vector2(-10, 7));
        anim.TrackInsertKey(t, 0.8f, new Vector2(-10, 7));

        // Waist tilts over
        t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, "Visuals/Waist:rotation");
        anim.TrackSetInterpolationType(t, Animation.InterpolationType.Cubic);
        anim.TrackInsertKey(t, 0f, 0f);
        anim.TrackInsertKey(t, 0.4f, -0.5f);
        anim.TrackInsertKey(t, 0.8f, -0.5f);

        // Fade out body parts
        t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, "Visuals/Waist:modulate");
        anim.TrackInsertKey(t, 0f, new Color(1, 1, 1, 1));
        anim.TrackInsertKey(t, 0.6f, new Color(1, 1, 1, 0.3f));
        anim.TrackInsertKey(t, 0.8f, new Color(1, 1, 1, 0));

        // Show corpse
        t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, "Visuals/Corpse:visible");
        anim.TrackInsertKey(t, 0f, false);
        anim.TrackInsertKey(t, 0.5f, true);

        // Corpse fade in
        t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, "Visuals/Corpse:modulate");
        anim.TrackInsertKey(t, 0f, new Color(1, 1, 1, 0));
        anim.TrackInsertKey(t, 0.5f, new Color(1, 1, 1, 0));
        anim.TrackInsertKey(t, 0.8f, new Color(1, 1, 1, 1));

        return anim;
    }
}
