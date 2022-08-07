using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MousekinRace
{
    public class ThoughtWorker_WearingPudgemouseFurApparel : ThoughtWorker
    {
        public override ThoughtState CurrentStateInternal(Pawn pawn)
        {
            // Only check for Mousekins
            if (!Utils.IsMousekin(pawn))
            {
                return ThoughtState.Inactive;
            }

            int num = 0;
            List<Apparel> wornApparel = pawn.apparel.WornApparel;
            for (int i = 0; i < wornApparel.Count; i++)
            {
                if (wornApparel[i].Stuff == MousekinDefOf.Mousekin_AnimalPudgemouse.race.leatherDef)
                {
                    num++;
                }
            }

            return (num > 0) ? ThoughtState.ActiveAtStage(0) : ThoughtState.Inactive;
        }
    }
}
