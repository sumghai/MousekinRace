using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MousekinRace
{
    [HarmonyPatch(typeof(BuildableDef), nameof(BuildableDef.SpecialDisplayStats))]
    public static class Harmony_BuildableDef_SpecialDisplayStats_PatchForMousekinXmasTree
    {
        static void Postfix(BuildableDef __instance, ref IEnumerable<StatDrawEntry> __result)
        {
            if (__instance == MousekinDefOf.Mousekin_IdeoXmasTree) 
            {
                __result = PatchCostList(__instance, __result);
            }
        }

        static IEnumerable<StatDrawEntry> PatchCostList(BuildableDef __instance, IEnumerable<StatDrawEntry> __result)
        {
            foreach (StatDrawEntry drawEntry in __result)
            {
                if (drawEntry.labelInt == "Stat_Building_ResourcesToMake".Translate())
                {
                    drawEntry.valueStringInt = drawEntry.valueStringInt.PatchMinifiedTreeWithPineTreeSuffix();
                }

                yield return drawEntry;
            }
        }
    }
}
