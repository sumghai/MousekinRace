using Verse;

namespace MousekinRace
{
    public class CompProperties_StateDependentBuildingEmissive : CompProperties
    {
        public GraphicData emissiveGraphicData = null;

        public CompProperties_StateDependentBuildingEmissive()
        {
            compClass = typeof(CompStateDependentBuildingEmissive);
        }
    }
}