using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MousekinRace
{
    public class IdeoMemeRitualsExtension : DefModExtension
    {
        public List<RitualWithExtraParams> ritualsWithExtraParams = new();
    }

    public class RitualWithExtraParams
    {
        public RequiredRitualAndBuilding ritual;

        public List<MemeDef> conflictingMemes;
    }
}
