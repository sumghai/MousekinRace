using HarmonyLib;
using RimWorld;

namespace MousekinRace
{
    // Set the outcome of fun/unforgettable Mousekin Tree Festival rituals to a fixed type
    // (currently 50% chance of new recruitee, may change to plant-related buff in the future)
    [HarmonyPatch(typeof(RitualPatternDef), nameof(RitualPatternDef.Fill))]
    public static class Harmony_RitualPatternDef_Fill_TreeFestivalRitualSetOutcome
    {
        static void Postfix(Precept_Ritual ritual)
        {
            if (ritual.ideo.culture.IsMousekin() && ritual.def.ritualPatternBase == MousekinDefOf.CelebrationTree)
            {
                ritual.attachableOutcomeEffect = MousekinDefOf.RandomRecruit;
            }
        }
    }
}
