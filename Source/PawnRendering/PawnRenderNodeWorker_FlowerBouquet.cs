using RimWorld;
using Verse;

namespace MousekinRace
{
    public class PawnRenderNodeWorker_FlowerBouquet : PawnRenderNodeWorker_Body
    {
        public override bool ShouldListOnGraph(PawnRenderNode node, PawnDrawParms parms)
        {
            return IsPerformingFlowerDanceNow(parms);
        }

        public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
        {
            if (!base.CanDrawNow(node, parms))
            {
                return false;
            }
            return IsPerformingFlowerDanceNow(parms);
        }

        public bool IsPerformingFlowerDanceNow(PawnDrawParms parms)
        { 
            return parms.pawn.CurJob?.def == MousekinDefOf.Mousekin_Job_PerformFlowerDance;
        }
    }
}
