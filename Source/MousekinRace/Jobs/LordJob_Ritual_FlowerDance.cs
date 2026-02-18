using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MousekinRace
{
    public class LordJob_Ritual_FlowerDance : LordJob_Ritual
    {
        public List<Pawn> nuns = new List<Pawn>();

        public LordJob_Ritual_FlowerDance() 
        {
        }

        public LordJob_Ritual_FlowerDance(TargetInfo selectedTarget, Precept_Ritual ritual, RitualObligation obligation, List<RitualStage> allStages, RitualRoleAssignments assignments, Pawn organizer = null, IntVec3? spotOverride = null)
        : base(selectedTarget, ritual, obligation, allStages, assignments, organizer, spotOverride)
        {
            foreach (RitualRole role in assignments.AllRolesForReading)
            {
                if (role != null && role.id.Contains("mousekinNun"))
                {
                    foreach (Pawn pawn in assignments.AssignedPawns(role))
                    { 
                        nuns.Add(pawn);
                    }
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref nuns, "nuns", LookMode.Reference);
        }
    }
}
