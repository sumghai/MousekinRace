using RimWorld;
using Verse;
using Verse.AI;

namespace MousekinRace
{
    public class WorkGiver_EmptyAshesFromHearth : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForGroup(ThingRequestGroup.Refuelable);

        public override PathEndMode PathEndMode => PathEndMode.Touch;

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (t.IsBurning())
            {
                return false;
            }
            if (t.IsForbidden(pawn) || !pawn.CanReserve(t, 1, -1, null, forced))
            {
                return false;
            }
            if (t.TryGetComp<CompAshMaker>() is CompAshMaker compAshMaker)
            {
                return compAshMaker.IsFull;
            }
            return false;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return JobMaker.MakeJob(MousekinDefOf.Mousekin_Job_EmptyAshesFromHearth, t);
        }
    }
}
