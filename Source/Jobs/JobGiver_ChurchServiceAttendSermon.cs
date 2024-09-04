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
            if ((duty.spectateRectPreferredSide == SpectateRectSide.None || !SpectatorCellFinder.TryFindSpectatorCellFor(pawn, duty.spectateRect, pawn.Map, out spot, duty.spectateRectPreferredSide)) && !SpectatorCellFinder.TryFindSpectatorCellFor(pawn, duty.spectateRect, pawn.Map, out spot, duty.spectateRectAllowedSides))
            {
                // todo - pick a spot that is either a sittable furniture (pew) or a "reasonable" standing spot (i.e. not right up against the lectern)
                IntVec3 target = duty.spectateRect.CenterCell;
                if (CellFinder.TryFindRandomReachableNearbyCell(target, pawn.MapHeld, 5f, TraverseParms.For(pawn), (IntVec3 c) => c.GetRoom(pawn.MapHeld) == target.GetRoom(pawn.MapHeld) && pawn.CanReserveSittableOrSpot(c) && !duty.spectateRect.Contains(c), null, out spot))
                {
                    return true;
                }
                Log.Warning("Failed to find a spectator spot for " + pawn);
                return false;
            }
            return true;
        }
    }
}
