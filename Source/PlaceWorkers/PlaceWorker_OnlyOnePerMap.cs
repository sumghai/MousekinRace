using RimWorld;
using Verse;

namespace MousekinRace
{
    public class PlaceWorker_OnlyOnePerMap : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            if (map.listerThings.ThingsOfDef((ThingDef)checkingDef).Count > 0 || map.listerThings.ThingsOfDef((ThingDef)checkingDef.blueprintDef).Count > 0 || map.listerThings.ThingsOfDef((ThingDef)checkingDef.frameDef).Count > 0)
            {
                return new AcceptanceReport("MousekinRace_PlaceWorker_OnlyOnePerMap".Translate(checkingDef.LabelCap));
            }

            return true;
        }
    }
}
