using System.Collections.Generic;
using Verse;

namespace MousekinRace
{
    // Custom placeworker to prevent Root Cellars from overlapping each other, other buildings, blueprints or frames
    // (required as Root Cellars are considered FloorEmplacement, which by default allows overlap)
    public class PlaceWorker_CellarOutdoor : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef def, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            foreach (IntVec3 item in GenAdj.OccupiedRect(loc, rot, def.Size))
            {                
                List<Thing> list = map.thingGrid.ThingsListAt(item);
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] != thingToIgnore && (list[i].def.IsEdifice() || list[i].def.IsBlueprint || list[i].def.IsFrame))
                    {
                        return "SpaceAlreadyOccupied".Translate();
                    }
                }
            }
            return true;
        }
    }
}
