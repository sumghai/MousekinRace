using HarmonyLib;
using RimWorld;

namespace MousekinRace
{
    // Conditionally override various field/parameter values for Mousekin ritual patterns
    // - Replace namemakers for funerals depending on Mousekin ideology
    // - Set the outcome of fun/unforgettable Mousekin rituals to a fixed type
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
                
                // todo - replace with plant-related work buff
                if (ritual.nameMaker == MousekinDefOf.NamerRitualTreeFestivalMousekin)
                {
                    ritual.attachableOutcomeEffect = MousekinDefOf.RandomRecruit;
                }

                if (ritual.nameMaker == MousekinDefOf.NamerRitualBarbecueMousekin)
                {
                    ritual.attachableOutcomeEffect = MousekinDefOf.RandomRecruit;
                }
            }
        }
    }
}
