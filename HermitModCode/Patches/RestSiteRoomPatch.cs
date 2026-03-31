using System.Linq;
using Godot;
using HarmonyLib;
using HermitMod.Character;
using MegaCrit.Sts2.Core.Nodes.RestSite;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace HermitMod.Patches;

[HarmonyPatch(typeof(NRestSiteRoom), "_Ready")]
public static class RestSiteRoomPatch
{
    [HarmonyPostfix]
    private static void Postfix(NRestSiteRoom __instance)
    {
        try
        {
            foreach (var character in __instance.Characters)
            {
                if (character?.Player?.Character is not Hermit)
                    continue;

                // Hide any SpineSprite children
                foreach (var child in character.GetChildren().OfType<Node2D>())
                {
                    if (child.GetClass() == "SpineSprite")
                        child.Visible = false;
                }

                var restSite = HermitRestSiteBuilder.Build();
                character.AddChild(restSite);
            }
        }
        catch
        {
        }
    }
}
