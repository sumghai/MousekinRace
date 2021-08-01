using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace MousekinRace
{
    public abstract class JobDriver_CommitSuicide : JobDriver
    {
        private const int TicksToPrepare = 100;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            pawn.Map.pawnDestinationReservationManager.Reserve(pawn, job, job.targetA.Cell);
            return true;
        }

        public override IEnumerable<Toil> MakeNewToils()
        {          
            LocalTargetInfo lookAtTarget = job.GetTarget(TargetIndex.B);
            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);
            yield return Toils_General.Wait(TicksToPrepare).WithProgressBarToilDelay(TargetIndex.B);

            Toil commitSuicide = new Toil();

            // For one-off actions (e.g. drinking poison)
            commitSuicide.initAction = delegate
            {
                ApplySuicideInjurySingle(pawn);
            };

            // For repetitive actions (e.g. self-stabbing)
            commitSuicide.tickAction = delegate
            {
                if (pawn.IsHashIntervalTick(50))
                {
                    ApplySuicideInjuryRepeatable(pawn);
                }
            };
            commitSuicide.defaultCompleteMode = ToilCompleteMode.Never;
            yield return commitSuicide;
        }

        public virtual void ApplySuicideInjurySingle(Pawn pawn)
        {
        }

        public virtual void ApplySuicideInjuryRepeatable(Pawn pawn)
        {
        }
    }
}
