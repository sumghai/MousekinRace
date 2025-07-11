using Verse;

namespace MousekinRace
{
    public class CompProperties_MineEntranceAnim : CompProperties
    {
        public GraphicData towerGraphicData;
        
        public GraphicData wheelSegmentGraphicData;

        public float wheelPairOffset = 1f;

        public float wheelDiameter = 1f;

        public int wheelSegmentsPerWheel = 1;


        public CompProperties_MineEntranceAnim() 
        {
            compClass = typeof(CompMineEntranceAnim);
        }
    }
}
