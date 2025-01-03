using HarmonyLib;
using RimWorld;

namespace MousekinRace
{
    // For Great Pines (Mousekin Xmas Trees), modify the cost list tip string
    // to state that it specifically requires a minified Pine tree
    [HarmonyPatch(typeof(Precept_Building), nameof(Precept_Building.CostListString))]
    public static class Harmony_Precept_Building_CostListString_PatchForMousekinXmasTree
    {
        static void Postfix(ref Precept_Building __instance, ref string __result)
        {
            if (__instance.thingDef == MousekinDefOf.Mousekin_IdeoXmasTree) 
            {
                __result = __result.PatchMinifiedTreeWithPineTreeSuffix();
            }
        }
    }
}
