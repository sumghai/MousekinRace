using RimWorld;
using Verse.AI.Group;
using Verse;
using Verse.AI;
using System.Linq;

namespace MousekinRace
{
    public class RitualBehaviorWorker_HereticExecution : RitualBehaviorWorker
    {
        public RitualBehaviorWorker_HereticExecution()
        {
        }

        public RitualBehaviorWorker_HereticExecution(RitualBehaviorDef def) : base(def)
        {
        }

        public override LordJob CreateLordJob(TargetInfo target, Pawn organizer, Precept_Ritual ritual, RitualObligation obligation, RitualRoleAssignments assignments)
        {
            return new LordJob_Ritual_HereticExecution(target, ritual, obligation, def.stages, assignments);
        }

        public override void PostCleanup(LordJob_Ritual ritual)
        {
            LordJob_Ritual_HereticExecution hereticExecution = ritual as LordJob_Ritual_HereticExecution;

            foreach (Pawn heretic in hereticExecution.heretics)
            {
                if (!heretic.Dead && !heretic.IsPrisonerInPrisonCell())
                {
                    Pawn escort = hereticExecution.escorts[hereticExecution.heretics.IndexOf(heretic)];

                    Job job = WorkGiver_Warden_TakeToBed.TryMakeJob(escort, heretic, forced: true);
                    if (job != null)
                    {
                        escort.jobs.StartJob(job, JobCondition.InterruptForced);
                    }
                    else
                    {
                        while (job == null)
                        {
                            Pawn backupEscort = hereticExecution.assignments.Participants.Where(p => p.DevelopmentalStage == DevelopmentalStage.Adult && !p.IsHeretic()).RandomElement();
                            job = WorkGiver_Warden_TakeToBed.TryMakeJob(backupEscort, heretic, forced: true);
                            backupEscort.jobs.StartJob(job, JobCondition.InterruptForced);
                        }
                    }
                    heretic.guest.WaitInsteadOfEscapingFor(1250);
                }
            }
        }
    }
}
