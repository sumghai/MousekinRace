using UnityEngine;
using Verse;

namespace MousekinRace
{
    public class PlaceWorker_Windmill : PlaceWorker
    {
        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
        {
            if (def.GetCompProperties<CompProperties_Windmill>() != null)
            {
                GenDraw.DrawRadiusRing(center, def.GetCompProperties<CompProperties_Windmill>().obstructionFreeRadius);
                GenDraw.DrawRadiusRing(center, def.GetCompProperties<CompProperties_Windmill>().terraformRadius, Color.gray);
            }
        }

        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            float radius = ((ThingDef)checkingDef).GetCompProperties<CompProperties_Windmill>().obstructionFreeRadius;

            if (CompWindmill.HasOtherWindmillOrBlueprintWithinRadius(loc, map, radius))
            {
                return new AcceptanceReport("MousekinRace_PlaceWorker_MustBeAwayFromOtherWindmills".Translate(checkingDef.LabelCap, radius - 1));
            }

            if (CompWindmill.HasObstructedCellsWithinRadius(loc, map, radius))
            {
                return new AcceptanceReport("MousekinRace_PlaceWorker_MustBeAwayFromOtherStructures".Translate(checkingDef.LabelCap, radius - 1));
            }

            return true;
        }
    }
}
