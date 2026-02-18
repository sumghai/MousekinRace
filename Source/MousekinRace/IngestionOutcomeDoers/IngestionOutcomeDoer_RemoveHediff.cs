using RimWorld;
using Verse;

namespace MousekinRace
{
    public class IngestionOutcomeDoer_RemoveHediff : IngestionOutcomeDoer
    {
        public HediffDef hediffDef;

        public override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested, int ingestedCount)
        {
            if (hediffDef != null) 
            {
                pawn.health.hediffSet.hediffs.RemoveAll(x => x.def == hediffDef);
            }
        }
    }
}
