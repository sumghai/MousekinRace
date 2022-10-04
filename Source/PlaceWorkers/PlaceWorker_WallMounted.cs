using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    public class PlaceWorker_WallMounted : PlaceWorker
    {
        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
        {
            IntVec3 wallCell = center + rot.FacingCell;
            GenDraw.DrawFieldEdges(new List<IntVec3>() { wallCell });
        }

        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            IntVec3 wallCell = loc + rot.FacingCell;
            if (loc.Impassable(map) || !wallCell.InBounds(map) || !wallCell.Impassable(map))
            {
                return new AcceptanceReport("MousekinRace_PlaceWorker_MustBeWallMounted".Translate());
            }
            if (loc.GetDoor(map) != null)
            {
                return new AcceptanceReport("MousekinRace_PlaceWorker_CannotPlaceInDoorways".Translate());
            }
            return true;
        }
    }
}
