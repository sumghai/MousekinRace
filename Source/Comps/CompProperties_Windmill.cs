using Verse;

namespace MousekinRace
{
    public class CompProperties_Windmill : CompProperties
    {
        public int obstructionFreeRadius = 10;

        public int affectedTerrainRadius = 10;

        public CompProperties_Windmill()
        {
            compClass = typeof(CompWindmill);
        }
    }
}
