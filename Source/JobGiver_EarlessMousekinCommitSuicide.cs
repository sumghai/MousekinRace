using RimWorld;
using Verse;
using Verse.AI;

namespace MousekinRace
{
    public class JobGiver_EarlessMousekinCommitSuicide : ThinkNode_JobGiver
    {
        public override Job TryGiveJob(Pawn pawn)
        {
            Log.Warning("Trying to give suicide job...");

            // Non-Mousekins should never attempt suicide over losing both ears
            if (!Utils.IsMousekin(pawn))
            {
                return null;
            }

            Log.Warning("Passed race check");

            MentalState_EarlessSuicide mentalState_EarlessSuicide = pawn.MentalState as MentalState_EarlessSuicide;

            if (mentalState_EarlessSuicide == null)
            {
                Log.Warning("Cannot give suicide job - ");
                return null;
            }

            

            Job job = JobMaker.MakeJob(JobDefOf.Vomit);
            job.count = 1;
            return job;
        }
    }
}
