using HarmonyLib;
using HermitMod.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Random;

namespace HermitMod.Patches;

[HarmonyPatch(typeof(CardCmd), nameof(CardCmd.Transform), [typeof(IEnumerable<CardTransformation>), typeof(Rng), typeof(CardPreviewStyle)])]
public static class StraightRazorTransformPatch
{
    [HarmonyPrefix]
    public static void Prefix(ref IEnumerable<CardTransformation> transformations)
    {
        CardTransformation[] transformationArray = transformations.ToArray();
        transformations = transformationArray;

        foreach (CardTransformation transformation in transformationArray)
        {
            if (transformation.Original.Pile?.Type == PileType.Deck)
                transformation.Original.Owner.GetRelic<StraightRazor>()?.Heal().GetAwaiter().GetResult();
        }
    }
}
