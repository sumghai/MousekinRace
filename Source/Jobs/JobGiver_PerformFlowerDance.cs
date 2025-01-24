using Verse;
using Verse.AI;

namespace MousekinRace
{
    public class JobGiver_PerformFlowerDance : ThinkNode_JobGiver
    {
        public override Job TryGiveJob(Pawn pawn)
        {
            PawnDuty duty = pawn.mindState.duty;

            if (duty == null)
            {
                return null;
            }

            return JobMaker.MakeJob(MousekinDefOf.Mousekin_Job_PerformFlowerDance);
        }
    }
}
