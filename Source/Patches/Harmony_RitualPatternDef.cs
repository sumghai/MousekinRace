using HarmonyLib;
using RimWorld;

namespace MousekinRace
{
    // Conditionally override various field/parameter values for Mousekin ritual patterns
    // - Replace namemakers for funerals depending on Mousekin ideology
    // - Set the outcome of fun/unforgettable Mousekin Tree Festival rituals to a fixed type
    //   (currently 50% chance of new recruitee, may change to plant-related buff in the future)
    [HarmonyPatch(typeof(RitualPatternDef), nameof(RitualPatternDef.Fill))]
    public static class Harmony_RitualPatternDef_Fill_OverrideValues
    {
        static void Postfix(Precept_Ritual ritual)
        {
            if (ritual.ideo.culture.IsMousekin())
            {
                if (ritual.def == PreceptDefOf.Funeral) 
                {
                    if (ritual.ideo.culture.IsMousekinIndyTownLike())
                    {
                        ritual.nameMaker = MousekinDefOf.NamerRitualFuneralMousekinIndyTown;
                    }
                    else if (ritual.ideo.culture.IsMousekinNomadLike())
                    {
                        ritual.nameMaker = MousekinDefOf.NamerRitualFuneralMousekinNomads;
                    }
                    else 
                    {
                        ritual.nameMaker = MousekinDefOf.NamerRitualFuneralMousekinKingdom;
                    }
                }
                
                if (ritual.def.ritualPatternBase == MousekinDefOf.CelebrationTree)
                {
                    ritual.attachableOutcomeEffect = MousekinDefOf.RandomRecruit;
                }
            }
        }
    }
}
