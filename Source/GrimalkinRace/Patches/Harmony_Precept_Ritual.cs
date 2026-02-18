using HarmonyLib;
using RimWorld;

namespace GrimalkinRace
{
    // Override Grimalkin ideo funeral ritual name to just "Funeral"
    [HarmonyPatch(typeof(Precept_Ritual), nameof(Precept_Ritual.GenerateNameRaw))]
    public static class Harmony_Precept_Ritual_GenerateNameRaw_OverrideGrimalkinFuneralRitualName
    {
        static bool Prefix(Precept_Ritual __instance, ref string __result, Ideo ___ideo)
        {
            if (___ideo.culture.IsGrimalkin() && __instance.def == PreceptDefOf.Funeral)
            {
                __result = __instance.def.label;
                return false;
            }
            return true;
        }
    }
}
