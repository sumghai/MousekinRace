using HarmonyLib;
using UnityEngine;
using Verse;

namespace MousekinRace.Patches
{
    // Force color of crafted apparel to be:
    // - Based on the apparel's color generators if the material/stuff permits color generators
    // - White if the apparel is specified to ignore stuff colors
    [HarmonyPatch(typeof(GenRecipe), nameof(GenRecipe.PostProcessProduct))]
    public static class Harmony_GenRecipe_PostProcessProduct_SetCraftedApparelColor
    {
        static void Prefix(ref Thing product)
        {
            CompColorable compColorable = product.TryGetComp<CompColorable>();

            CompApparelIgnoreStuffColor compApparelIgnoreStuffColor = product.TryGetComp<CompApparelIgnoreStuffColor>();

            if (compColorable != null)
            {
                if (product.def.colorGenerator != null && (product.Stuff == null || product.Stuff.stuffProps.allowColorGenerators))
                {
                    product.SetColor(product.def.colorGenerator.NewRandomizedColor());
                }
                else if (compApparelIgnoreStuffColor != null)
                {
                    product.SetColor(Color.white);
                }
            }
        }
    }
}
