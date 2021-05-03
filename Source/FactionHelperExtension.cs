using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MousekinRace
{
    public class FactionHelperExtension : DefModExtension
    {
        public bool canSpawnSettlements = true;

        public List<FactionDefStartingGoodwill> startingGoodwillByFactionDefs = new List<FactionDefStartingGoodwill>();

        public List<FactionDef> rivalFactionTypes = new List<FactionDef>();
    }
}
