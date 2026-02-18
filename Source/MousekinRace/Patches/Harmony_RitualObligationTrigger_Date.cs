using HarmonyLib;
using RimWorld;

namespace MousekinRace
{
    // Set the date of the Mousekin Tree Festival ritual to 10th Decembrary every year
    [HarmonyPatch(typeof(RitualObligationTrigger_Date), nameof(RitualObligationTrigger_Date.Init))]
    public static class Harmony_RitualObligationTrigger_Date_Init_TreeFestivalRitualSetDate
    {
        static void Postfix(RitualObligationTrigger_Date __instance)
        {
            if (__instance.ritual.ideo.culture.IsMousekin() && __instance.ritual.nameMaker == MousekinDefOf.NamerRitualTreeFestivalMousekin)
            {
                __instance.triggerDaysSinceStartOfYear = 54; // 10th Decembrary in-game
            }
        }
    }
}
