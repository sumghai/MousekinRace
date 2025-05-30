using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace MousekinRace
{
    public class WorkGiver_MineResourcesFromMineEntrance : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(MousekinDefOf.Mousekin_MineEntrance);

        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            List<Thing> list = pawn.Map.listerThings.ThingsOfDef(MousekinDefOf.Mousekin_MineEntrance);
            for (int i = 0; i < list.Count; i++)
            {
                if (((Building_MineEntrance)list[i]).compUndergroundMineDeposits.MiningBill.ShouldDoNow())
                {
                    return false;
                }
            }
            return true;
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (t is not Building_MineEntrance building_MineEntrance || !building_MineEntrance.compUndergroundMineDeposits.MiningBill.ShouldDoNow())
            {
                return false;
            }
            if (t.IsBurning())
            {
                return false;
            }
            return true;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return JobMaker.MakeJob(MousekinDefOf.Mousekin_Job_MineResourcesFromMineEntrance, t);
        }
    }
}
