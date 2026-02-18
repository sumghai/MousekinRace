using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MousekinRace
{
    public class RitualObligationTargetWorker_TownSquare : RitualObligationTargetWorker_AnyGatherSpot
    {
        public RitualObligationTargetWorker_TownSquare()
        {
        }

        public RitualObligationTargetWorker_TownSquare(RitualObligationTargetFilterDef def)
        : base(def)
        {
        }

        public override IEnumerable<TargetInfo> GetTargets(RitualObligation obligation, Map map)
        {
            List<Thing> townSquares = map.listerThings.ThingsOfDef(MousekinDefOf.Mousekin_TownSquare);
            for (int i = 0; i < townSquares.Count; i++)
            {
                yield return townSquares[i];
            }
        }

        public override RitualTargetUseReport CanUseTargetInternal(TargetInfo target, RitualObligation obligation)
        {
            if (!target.HasThing)
            {
                return false;
            }
            return target.Thing.def == MousekinDefOf.Mousekin_TownSquare;
        }

        public override IEnumerable<string> GetTargetInfos(RitualObligation obligation)
        {
            yield return MousekinDefOf.Mousekin_TownSquare.label;
        }
    }
}
