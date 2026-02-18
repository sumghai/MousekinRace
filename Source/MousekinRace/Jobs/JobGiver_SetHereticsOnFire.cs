using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace MousekinRace
{
    public class JobGiver_SetHereticsOnFire : ThinkNode_JobGiver
    {
        public override Job TryGiveJob(Pawn pawn)
        {
            LordJob_Ritual_HereticExecution hereticExecution = pawn.GetLord()?.LordJob as LordJob_Ritual_HereticExecution;
            int hereticEscortPairingIndex = hereticExecution.escorts.FindIndex(p => p == pawn);
            Thing hereticForThisEscort = hereticExecution.heretics[hereticEscortPairingIndex];
            if (!pawn.CanReserveAndReach(hereticForThisEscort, PathEndMode.ClosestTouch, Danger.None))
            {
                return null;
            }
            return JobMaker.MakeJob(MousekinDefOf.Mousekin_Job_SetHereticOnFire, hereticForThisEscort);
        }
    }
}
