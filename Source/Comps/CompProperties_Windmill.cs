using Verse;

namespace MousekinRace
{
    public class CompProperties_Windmill : CompProperties
    {
        public float obstructionFreeRadius = 10;

        public float terraformRadius = 10;

        public float daysToTerraformRadius = 5;

        public GraphicData capGraphicData;

        public GraphicData sailGraphicData;

        public int sailCount = 4;

        public CompProperties_Windmill()
        {
            compClass = typeof(CompWindmill);
        }
    }
}
