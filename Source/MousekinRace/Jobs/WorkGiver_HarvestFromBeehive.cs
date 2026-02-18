using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace MousekinRace
{
    public class WorkGiver_HarvestFromBeehive : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(MousekinDefOf.Mousekin_Beehive);

        public override PathEndMode PathEndMode => PathEndMode.Touch;

        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            List<Thing> list = pawn.Map.listerThings.ThingsOfDef(MousekinDefOf.Mousekin_Beehive);
            for (int i = 0; i < list.Count; i++)
            {
                if (((Building_Beehive)list[i]).ProductsReady)
                {
                    return false;
                }
            }
            return true;
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (!(t is Building_Beehive building_Beehive) || !building_Beehive.ProductsReady)
            {
                return false;
            }
            if (t.IsBurning())
            {
                return false;
            }
            if (t.IsForbidden(pawn) || !pawn.CanReserve(t, 1, -1, null, forced))
            {
                return false;
            }
            return true;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return JobMaker.MakeJob(MousekinDefOf.Mousekin_Job_HarvestFromBeehive, t);
        }
    }
}
