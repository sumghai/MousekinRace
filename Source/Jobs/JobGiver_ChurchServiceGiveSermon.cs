using Verse;
using Verse.AI;

namespace MousekinRace
{
    public class JobGiver_ChurchServiceGiveSermon : ThinkNode_JobGiver
    {
        public override Job TryGiveJob(Pawn pawn)
        {
            PawnDuty duty = pawn.mindState.duty;

            if (duty == null)
            { 
                return null;
            }

            if (duty.focusSecond.Thing is not Building churchLectern || !pawn.CanReach((LocalTargetInfo)churchLectern, PathEndMode.InteractionCell, Danger.None, false, false, TraverseMode.ByPawn))
            {
                return null;
            }

            return JobMaker.MakeJob(MousekinDefOf.Mousekin_Job_ChurchServiceGiveSermon, duty.focusSecond);
        }
    }
}
