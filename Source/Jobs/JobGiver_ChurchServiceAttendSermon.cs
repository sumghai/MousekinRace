using RimWorld;
using System.Linq;
using Verse;
using Verse.AI;

namespace MousekinRace
{
    public class JobGiver_ChurchServiceAttendSermon : ThinkNode_JobGiver
    {
        public override Job TryGiveJob(Pawn pawn)
        {
            PawnDuty duty = pawn.mindState.duty;

            if (duty == null)
            {
                return null;
            }

            if (!TryFindSpot(pawn, duty, out var spot))
            {
                return null;
            }
            
            Building churchLectern = spot.GetRoom(pawn.MapHeld).ContainedThings(MousekinDefOf.Mousekin_ChurchLectern).FirstOrDefault() as Building;
            Building edifice = spot.GetEdifice(pawn.Map);

            if (edifice != null && pawn.CanReserveSittableOrSpot(spot))
            {
                return JobMaker.MakeJob(MousekinDefOf.Mousekin_Job_ChurchServiceAttendSermon, spot, churchLectern, edifice);
            }
            return JobMaker.MakeJob(MousekinDefOf.Mousekin_Job_ChurchServiceAttendSermon, spot, churchLectern);
        }

        public virtual bool TryFindSpot(Pawn pawn, PawnDuty duty, out IntVec3 spot)
        {           
            if ((duty.spectateRectPreferredSide == SpectateRectSide.None || !SpectatorCellFinder.TryFindSpectatorCellFor(pawn, duty.spectateRect, pawn.Map, out spot, duty.spectateRectPreferredSide, extraValidator: GoodSpectateCellForChurchSermon)) && !SpectatorCellFinder.TryFindSpectatorCellFor(pawn, duty.spectateRect, pawn.Map, out spot, duty.spectateRectAllowedSides, extraValidator: GoodSpectateCellForChurchSermon))
            {
                return false;
            }
            return spot.IsValid;
        }

        public static bool GoodSpectateCellForChurchSermon(IntVec3 spot, Pawn p, Map map)
        {
            // Prioritize looking for seats
            Building potentialSeat = spot.GetEdifice(p.Map);
            bool spotIsSeat = potentialSeat != null && potentialSeat.def.category == ThingCategory.Building && potentialSeat.def.building.isSittable;

            // Fallback: free standing spot behind last row of seats/pews
            Building lectern = spot.GetRoom(p.MapHeld).ContainedThings(MousekinDefOf.Mousekin_ChurchLectern).FirstOrDefault() as Building;
            bool spotBehindSeatsIsStandable = ChurchService_Utils.GetStandableCellsBehindPews(map, lectern).Contains(spot);

            return spotIsSeat || spotBehindSeatsIsStandable;
        }
    }
}
