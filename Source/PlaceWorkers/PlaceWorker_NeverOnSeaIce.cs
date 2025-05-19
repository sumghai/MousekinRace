using RimWorld;
using Verse;

namespace MousekinRace
{
    public class PlaceWorker_NeverOnSeaIce : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            if (loc.GetTerrain(map) == TerrainDefOf.Ice)
            { 
                return "MousekinRace_PlaceWorker_NeverOnSeaIce".Translate(checkingDef.LabelCap);
            }
            return true;
        }
    }
}
