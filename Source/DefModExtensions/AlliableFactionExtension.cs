using Verse;

namespace MousekinRace
{
    public class AlliableFactionExtension : DefModExtension
    {
        public AlliableFactionJoinRequirements joinRequirements = new();
        
        [MustTranslate]
        public string joinButtonLabel;
    }

    public class AlliableFactionJoinRequirements 
    {
        public int minDaysPassedSinceSettle = 60;
        public float minMousekinPopulationPercentage = 0.75f;
        public int minGoodwill = 50;
    }
}
