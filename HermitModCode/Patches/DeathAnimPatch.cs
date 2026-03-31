using System;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace HermitMod.Patches;

/// <summary>
/// Fades out the character visuals and shows a corpse sprite on death.
/// </summary>
[HarmonyPatch(typeof(NCreature), "StartDeathAnim")]
public static class DeathAnimPatch
{
    [HarmonyPostfix]
    public static void Postfix(NCreature __instance)
    {
        Node val = ((Node)__instance).FindChild("Corpse", true, false);
        Sprite2D corpseSprite = (Sprite2D)(object)((val is Sprite2D) ? val : null);
        if (corpseSprite == null)
            return;

        Node val2 = ((Node)__instance).FindChild("Visuals", true, false);
        Node2D visualsNode = (Node2D)(object)((val2 is Node2D) ? val2 : null);
        if (visualsNode != null)
        {
            Tween tween = ((Node)__instance).CreateTween();
            tween.TweenProperty((GodotObject)(object)visualsNode, new NodePath("modulate:a"), Variant.From(0f), 0.5);
            tween.TweenCallback(Callable.From((Action)delegate
            {
                ((CanvasItem)visualsNode).Visible = false;
                ((CanvasItem)corpseSprite).Visible = true;
                ((CanvasItem)corpseSprite).Modulate = new Color(1f, 1f, 1f, 0f);
            }));
            tween.TweenProperty((GodotObject)(object)corpseSprite, new NodePath("modulate:a"), Variant.From(1f), 0.30000001192092896);
        }
    }
}
