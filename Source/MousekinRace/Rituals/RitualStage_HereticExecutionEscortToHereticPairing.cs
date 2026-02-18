using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MousekinRace
{
    public class RitualStage_HereticExecutionEscortToHereticPairing : RitualStage
    {
        public override IEnumerable<RitualStagePawnSecondFocus> GetPawnSecondFoci(LordJob_Ritual ritual)
        {
            LordJob_Ritual_HereticExecution hereticExecution = ritual as LordJob_Ritual_HereticExecution;

            foreach (Pawn escort in hereticExecution.escorts)
            {
                yield return new RitualStagePawnSecondFocus
                {
                    pawn = escort,
                    target = hereticExecution.heretics[hereticExecution.escorts.FindIndex(p => p == escort)]
                };
            }
        }
    }
}
