using HarmonyLib;
using RimWorld;
using Verse;

namespace MousekinRace
{
    [HarmonyPatch(typeof(Blueprint_Build), nameof(Blueprint_Build.GetInspectString))]
    public class Harmony_Blueprint_Build_GetInspectString_PatchForMousekinXmasTree
    {
        static void Postfix(Blueprint_Build __instance, ref string __result)
        {
            if (__instance.BuildDef == MousekinDefOf.Mousekin_IdeoXmasTree)
            {                
                __result = __result.PatchMinifiedTreeWithPineTreeSuffix();
            }
        }
    }
}
