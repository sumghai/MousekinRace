using HarmonyLib;
using RimWorld;

namespace MousekinRace
{
    [HarmonyPatch(typeof(Frame), nameof(Frame.GetInspectString))]
    public class Harmony_Frame_GetInspectString_PatchForMousekinXmasTree
    {
        static void Postfix(Frame __instance, ref string __result)
        {
            if (__instance.BuildDef == MousekinDefOf.Mousekin_IdeoXmasTree)
            {                
                __result = __result.PatchMinifiedTreeWithPineTreeSuffix();
            }
        }
    }
}
