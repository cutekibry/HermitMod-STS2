using System;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace HermitMod.Patches;

/// <summary>
/// Tilts the character during attack animations for a stylistic effect.
/// </summary>
[HarmonyPatch(typeof(NCreature), "SetAnimationTrigger")]
public static class HermitAttackTiltPatch
{
    private const float TiltRadians = -0.26f;

    [HarmonyPrefix]
    public static bool Prefix(NCreature __instance, string triggerName)
    {
        if (!string.Equals(triggerName, "Attack", StringComparison.OrdinalIgnoreCase))
            return true;

        Node obj = ((Node)__instance).FindChild("AttackTilt", true, false);
        Node2D tiltNode = (Node2D)(object)((obj is Node2D) ? obj : null);
        if (tiltNode == null)
            return true;

        tiltNode.Rotation = 0f;
        Tween tween = ((Node)tiltNode).CreateTween();
        tween.TweenProperty((GodotObject)(object)tiltNode, new NodePath("rotation"), Variant.From(-0.26f), 0.1);
        tween.TweenProperty((GodotObject)(object)tiltNode, new NodePath("rotation"), Variant.From(0f), 0.4);
        return false;
    }
}
