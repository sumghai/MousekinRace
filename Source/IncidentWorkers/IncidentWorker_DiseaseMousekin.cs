using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MousekinRace
{
    public class IncidentWorker_DiseaseMousekin : IncidentWorker_DiseaseHuman
    {
        public override IEnumerable<Pawn> PotentialVictimCandidates(IIncidentTarget target)
        {
            if (target is Map map)
            {
                return map.mapPawns.FreeColonistsAndPrisoners.Where((Pawn x) => (x.ParentHolder == null || x.ParentHolder is not CompBiosculpterPod) && x.IsMousekin());
            }
            return ((Caravan)target).PawnsListForReading.Where((Pawn x) => (x.IsFreeColonist || x.IsPrisonerOfColony) && x.IsMousekin());
        }
    }
}
