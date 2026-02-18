using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MousekinRace
{
    // Only allows one building with this placeworker to be built on the current map;
    // - checks for already-placed blueprints and frames
    // - allows reinstalling already-built buildings elsewhere
    public class PlaceWorker_OnlyOnePerMap : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            List<Thing> things = map.listerThings.ThingsOfDef((ThingDef)checkingDef);
            List<Thing> blueprints = map.listerThings.ThingsOfDef(checkingDef.blueprintDef);
            List<Thing> frames = map.listerThings.ThingsOfDef(checkingDef.frameDef);

            // thingToIgnore gets set to the already-built building only when reinstalling
            if ((things.Count > 0 && !things.Contains(thingToIgnore)) || blueprints.Count > 0 || frames.Count > 0)
            {
                return new AcceptanceReport("MousekinRace_PlaceWorker_OnlyOnePerMap".Translate(checkingDef.LabelCap));
            }

            return true;
        }
    }
}
