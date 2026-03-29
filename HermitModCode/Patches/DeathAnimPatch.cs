using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace HermitMod.Patches;

/// <summary>
/// Ensures the death animation plays for custom (non-Spine) visuals.
/// The base game only calls SetAnimationTrigger("Dead") when _spineAnimator != null,
/// so custom visuals using a Godot AnimationPlayer never get the death trigger.
/// This patch calls SetAnimationTrigger("Dead") for non-Spine creatures.
/// BaseLib's CustomAnimationPatch then translates that trigger into AnimationPlayer.Play("die").
/// </summary>
[HarmonyPatch(typeof(NCreature), nameof(NCreature.StartDeathAnim))]
public static class DeathAnimPatch
{
    [HarmonyPostfix]
    public static void Postfix(NCreature __instance)
    {
        // Only trigger for non-Spine creatures (custom visuals)
        // Spine creatures already get the death animation from the original method
        if (__instance.HasSpineAnimation)
            return;

        // Trigger the death animation — BaseLib's CustomAnimationPatch
        // will intercept this and play "die" on the AnimationPlayer
        __instance.SetAnimationTrigger("Dead");
    }
}
