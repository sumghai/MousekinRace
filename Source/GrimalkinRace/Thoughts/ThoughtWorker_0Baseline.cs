using RimWorld;
using Verse;

namespace GrimalkinRace
{
    public class ThoughtWorker_0Baseline : ThoughtWorker
    {
        public override ThoughtState CurrentStateInternal(Pawn pawn)
        {
            if (pawn.IsGrimalkin())
            {
                return ThoughtState.ActiveAtStage((pawn.Faction.def == GrimalkinDefOf.Grimalkin_Faction) ? 1 : 0);
            }
            return ThoughtState.Inactive;
        }
    }
}
