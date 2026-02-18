using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MousekinRace
{
    public class RitualOutcomeComp_OneEscortPerHeretic : RitualOutcomeComp
    {
        public string hereticRoleId;

        public string escortRoleId;

        public override bool Applies(LordJob_Ritual ritual) => true;

        public override IEnumerable<string> BlockingIssues(Precept_Ritual ritual, TargetInfo target, RitualRoleAssignments assignments)
        {
            int numHeretics = assignments.AssignedPawns(hereticRoleId).Count();
            int numEscorts = assignments.AssignedPawns(escortRoleId).Count();

            if (numHeretics > 0 && numEscorts > 0) 
            {
                if (numHeretics > numEscorts)
                {
                    yield return "MousekinRace_MessageRitualOutcomeBlocked_TooManyHeretics".Translate();
                }
                if (numHeretics < numEscorts)
                {
                    yield return "MousekinRace_MessageRitualOutcomeBlocked_TooManyEscorts".Translate();
                }
            }
        }
    }
}
