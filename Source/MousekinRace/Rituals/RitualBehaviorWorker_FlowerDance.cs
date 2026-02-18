using RimWorld;
using Verse;
using Verse.AI.Group;

namespace MousekinRace
{
    public class RitualBehaviorWorker_FlowerDance : RitualBehaviorWorker
    {
        public RitualBehaviorWorker_FlowerDance()
        { 
        }

        public RitualBehaviorWorker_FlowerDance(RitualBehaviorDef def) : base(def) 
        { 
        }

        public override LordJob CreateLordJob(TargetInfo target, Pawn organizer, Precept_Ritual ritual, RitualObligation obligation, RitualRoleAssignments assignments)
        {
            return new LordJob_Ritual_FlowerDance(target, ritual, obligation, def.stages, assignments);
        }
    }
}
