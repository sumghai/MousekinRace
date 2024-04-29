using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MousekinRace
{
    public class AlliableFactionExtension : DefModExtension
    {
        public AlliableFactionJoinRequirements joinRequirements = new();
        
        [MustTranslate]
        public string joinButtonLabel;

        [MustTranslate]
        public string membershipTypeLabel;

        public List<FactionDef> hostileToFactionTypes = new();
    }

    public class AlliableFactionJoinRequirements 
    {
        public int minDaysPassedSinceSettle = 60;
        public float minMousekinPopulationPercentage = 0.75f;
        public int minGoodwill = 50;
    }
}
