using RimWorld;
using System.Collections.Generic;
using Verse;

namespace GrimalkinRace
{
    public class ThoughtWorker_WearingGrimalkinFurApparel : ThoughtWorker
    {
        public override ThoughtState CurrentStateInternal(Pawn pawn)
        {
            // Only check for Grimalkins
            if (!Utils.IsGrimalkin(pawn))
            {
                return ThoughtState.Inactive;
            }

            int num = 0;
            List<Apparel> wornApparel = pawn.apparel.WornApparel;
            for (int i = 0; i < wornApparel.Count; i++)
            {
                if (wornApparel[i].Stuff == GrimalkinDefOf.Grimalkin.race.leatherDef)
                {
                    num++;
                }
            }

            return (num > 0) ? ThoughtState.ActiveAtStage(0) : ThoughtState.Inactive;
        }
    }
}
