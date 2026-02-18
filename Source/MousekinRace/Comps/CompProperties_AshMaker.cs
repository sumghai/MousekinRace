using Verse;

namespace MousekinRace
{
    public class CompProperties_AshMaker : CompProperties
    {
        public ThingDef ashDef;

        public float ashCapacity = 5f;

        public float generationMultiplier = 0.5f;

        public CompProperties_AshMaker()
        {
            compClass = typeof(CompAshMaker);
        }
    }
}
