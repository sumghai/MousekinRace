using Verse;

namespace MousekinRace
{
    public class CompProperties_MineEntranceAnim : CompProperties
    {
        public GraphicData towerGraphicData;

        public GraphicData workGlowGraphicData;

        public GraphicData wheelSegmentGraphicData;

        public float wheelPairOffset = 1f;

        public float wheelDiameter = 1f;

        public int wheelSegmentsPerWheel = 1;

        public float wheelSpeed = 1f;

        public IntRange ticksToChangeWheelDirection = new(2500, 2500);


        public CompProperties_MineEntranceAnim() 
        {
            compClass = typeof(CompMineEntranceAnim);
        }
    }
}
