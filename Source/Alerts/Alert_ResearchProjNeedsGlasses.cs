using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MousekinRace
{
    public class Alert_ResearchProjNeedsGlasses : Alert
    {
        public Alert_ResearchProjNeedsGlasses()
        {
            defaultPriority = AlertPriority.High;
        }

        public override string GetLabel()
        {
            return "AlertResearchProjNeedsGlassesLabel".Translate(MousekinDefOf.Mousekin_GlassesLarge.LabelCap.Replace(MousekinDefOf.Mousekin.LabelCap, ""));
        }

        public override TaggedString GetExplanation()
        {
            return "AlertResearchProjNeedsGlassesDesc".Translate(
                Find.ResearchManager.GetProject().LabelCap,
                MousekinDefOf.Mousekin_GlassesLarge.LabelCap,
                MousekinDefOf.Mousekin_GlassesLarge.LabelCap.Replace(MousekinDefOf.Mousekin.LabelCap, ""),
                MousekinDefOf.Mousekin_WorkbenchCrafting.LabelCap
            );
        }

        public override AlertReport GetReport()
        {
            List<ResearchProjectDef> projects = MousekinDefOf.ResearchProjectsNeedingGlasses;

            bool mousekinWearingGlassesFound = false;
            
            if (MousekinDefOf.ResearchProjectsNeedingGlasses.Contains(Find.ResearchManager.GetProject()))
            {
                // Only check for pawns capable of and assigned to research
                foreach (Pawn pawn in PawnsFinder.AllMaps_FreeColonistsSpawned.Where(p => !p.Dead && p.workSettings.GetPriority(WorkTypeDefOf.Research) > 0))
                {
                    if (pawn.apparel.WornApparel.FirstOrDefault(ap => ap.def == MousekinDefOf.Mousekin_GlassesLarge) != null)
                    {
                        mousekinWearingGlassesFound = true;
                        break;
                    }
                }
                return !mousekinWearingGlassesFound;
            }
            return false;
        }
    }
}
