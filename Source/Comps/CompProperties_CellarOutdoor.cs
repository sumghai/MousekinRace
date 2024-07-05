using Verse;

namespace MousekinRace
{
    public class CompProperties_CellarOutdoor : CompProperties
    {
        public GraphicData exposedWallGraphicData = null;

        public CompProperties_CellarOutdoor() 
        {
            compClass = typeof(CompCellarOutdoor);
        }
    }
}
