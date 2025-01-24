using RimWorld;
using System.Collections.Generic;

namespace MousekinRace
{
    public class RitualOutcomeComp_DeathThoughts : RitualOutcomeComp
    {
        public List<ThoughtDef> thoughtDefsToRemoveOnPositiveOutcome;

        public override bool Applies(LordJob_Ritual ritual) => true;
    }
}
