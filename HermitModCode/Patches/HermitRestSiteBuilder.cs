using Godot;

namespace HermitMod.Patches;

public static class HermitRestSiteBuilder
{
    private const string CharDir = "res://HermitMod/images/char/";
    private const float WaistY = -20f;

    public static Node2D Build()
    {
        var root = new Node2D();
        root.Name = "HermitRestSite";
        root.Position = new Vector2(0f, 30f);
        root.Scale = new Vector2(1.8f, 1.8f);

        var shadow = CreateSprite("Shadow", CharDir + "shadow.png", new Vector2(0f, -5f));
        shadow.Scale = new Vector2(0.8f, 0.6f);
        root.AddChild(shadow);

        var visuals = new Node2D();
        visuals.Name = "Visuals";
        visuals.Scale = new Vector2(1.3f, 1.3f);
        root.AddChild(visuals);

        var waist = new Node2D();
        waist.Name = "Waist";
        waist.Position = new Vector2(2f, WaistY);
        visuals.AddChild(waist);

        var rightLeg = CreateSprite("RightLeg", CharDir + "leg_right.png", new Vector2(12f, 30f));
        rightLeg.Rotation = 0.7f;
        waist.AddChild(rightLeg);

        var leftLeg = CreateSprite("LeftLeg", CharDir + "leg_left.png", new Vector2(-10f, 32f));
        leftLeg.Rotation = 0.5f;
        waist.AddChild(leftLeg);

        var body = CreateSprite("Body", CharDir + "body.png", new Vector2(0f, -5f));
        body.Offset = new Vector2(0f, -50f);
        waist.AddChild(body);

        var rightArm = CreateSprite("RightArm", CharDir + "right_hand.png", new Vector2(32f, -60f));
        rightArm.Rotation = 0.5f;
        waist.AddChild(rightArm);

        var leftArm = new Node2D();
        leftArm.Name = "LeftArm";
        leftArm.Position = new Vector2(-30f, -65f);
        leftArm.Rotation = -0.6f;
        waist.AddChild(leftArm);

        var handGun = CreateSprite("HandGun", CharDir + "hand_gun.png", new Vector2(-5f, 5f));
        leftArm.AddChild(handGun);

        var gun = CreateSprite("Gun", CharDir + "gun.png", new Vector2(-12f, -12f));
        gun.Rotation = 0.8f;
        gun.Scale = new Vector2(0.85f, 0.85f);
        leftArm.AddChild(gun);

        var head = new Node2D();
        head.Name = "Head";
        head.Position = new Vector2(-2f, -100f);
        waist.AddChild(head);

        var hat = CreateSprite("Hat", CharDir + "hat.png", new Vector2(8f, -40f));
        head.AddChild(hat);

        var eye = CreateSprite("Eye", CharDir + "eye.png", new Vector2(18f, -10f));
        head.AddChild(eye);

        var animPlayer = CreateSeatedIdleAnimation();
        root.AddChild(animPlayer);
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

    private static AnimationPlayer CreateSeatedIdleAnimation()
    {
        var player = new AnimationPlayer();
        player.Name = "AnimPlayer";

        var anim = new Animation();
        anim.Length = 3f;
        anim.LoopMode = Animation.LoopModeEnum.Linear;

        // Waist position
        int t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, new NodePath("Visuals/Waist:position"));
        anim.TrackSetInterpolationType(t, Animation.InterpolationType.Cubic);
        anim.TrackInsertKey(t, 0.0, new Vector2(2f, WaistY));
        anim.TrackInsertKey(t, 1.5, new Vector2(2f, WaistY + 2f));
        anim.TrackInsertKey(t, 3.0, new Vector2(2f, WaistY));

        // Waist rotation
        t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, new NodePath("Visuals/Waist:rotation"));
        anim.TrackSetInterpolationType(t, Animation.InterpolationType.Cubic);
        anim.TrackInsertKey(t, 0.0, 0.04f);
        anim.TrackInsertKey(t, 1.5, 0.06f);
        anim.TrackInsertKey(t, 3.0, 0.04f);

        // Head position
        t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, new NodePath("Visuals/Waist/Head:position"));
        anim.TrackSetInterpolationType(t, Animation.InterpolationType.Cubic);
        anim.TrackInsertKey(t, 0.0, new Vector2(-2f, -100f));
        anim.TrackInsertKey(t, 1.5, new Vector2(-2f, -98.5f));
        anim.TrackInsertKey(t, 3.0, new Vector2(-2f, -100f));

        // Head rotation
        t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, new NodePath("Visuals/Waist/Head:rotation"));
        anim.TrackSetInterpolationType(t, Animation.InterpolationType.Cubic);
        anim.TrackInsertKey(t, 0.0, 0.05f);
        anim.TrackInsertKey(t, 1.5, 0.09f);
        anim.TrackInsertKey(t, 3.0, 0.05f);

        // Left arm rotation
        t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, new NodePath("Visuals/Waist/LeftArm:rotation"));
        anim.TrackSetInterpolationType(t, Animation.InterpolationType.Cubic);
        anim.TrackInsertKey(t, 0.0, -0.6f);
        anim.TrackInsertKey(t, 1.5, -0.57f);
        anim.TrackInsertKey(t, 3.0, -0.6f);

        // Right arm rotation
        t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, new NodePath("Visuals/Waist/RightArm:rotation"));
        anim.TrackSetInterpolationType(t, Animation.InterpolationType.Cubic);
        anim.TrackInsertKey(t, 0.0, 0.5f);
        anim.TrackInsertKey(t, 1.5, 0.53f);
        anim.TrackInsertKey(t, 3.0, 0.5f);

        var library = new AnimationLibrary();
        library.AddAnimation("idle", anim);
        player.AddAnimationLibrary("", library);

        return player;
    }
}
