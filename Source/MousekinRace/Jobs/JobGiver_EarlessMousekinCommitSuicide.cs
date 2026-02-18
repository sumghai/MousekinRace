using Verse;
using Verse.AI;

namespace MousekinRace
{
    public class JobGiver_EarlessMousekinCommitSuicide : ThinkNode_JobGiver
    {
        public override Job TryGiveJob(Pawn pawn)
        {
            // Non-Mousekins should never attempt suicide over losing both ears
            if (!Utils.IsMousekin(pawn))
            {
                return null;
            }

            MentalState_EarlessSuicide mentalState_EarlessSuicide = pawn.MentalState as MentalState_EarlessSuicide;

            // Don't attempt suicide if the the pawn doesn't have the Earless Suicide mental state, or if they cannot find/reach their target destination (i.e. their bed)
            if (mentalState_EarlessSuicide == null || !mentalState_EarlessSuicide.target.IsValid || !pawn.CanReach(mentalState_EarlessSuicide.target, PathEndMode.Touch, Danger.Deadly))
            {
                return null;
            }

            JobDef suicideJobChoice;

            // Mice incapable of violence (e.g. priests and nuns) will take poison, while other mice stab themselves 
            if (pawn.WorkTagIsDisabled(WorkTags.Violent))
            {
                suicideJobChoice = MousekinDefOf.Mousekin_Job_CommitSuicideWithPoison;
            }
            else
            {
                suicideJobChoice = MousekinDefOf.Mousekin_Job_CommitSuicideWithKnife;
            }

            Job job = JobMaker.MakeJob(suicideJobChoice, mentalState_EarlessSuicide.target);

            // Suicidal mice walk slowly and reluctantly to their bedroom, giving players time to intervene
            job.locomotionUrgency = LocomotionUrgency.Amble;
            job.count = 1;
            return job;
        }
    }
}
