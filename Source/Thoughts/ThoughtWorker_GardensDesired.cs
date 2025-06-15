using RimWorld;
using Verse;

namespace MousekinRace
{
    public class ThoughtWorker_GardensDesired : ThoughtWorker_Precept
    {
        public override ThoughtState ShouldHaveThought(Pawn p)
        {
            MapComponent_FlowerTracker flowerTracker = p.Map.GetComponent<MapComponent_FlowerTracker>();
            int gardenSize = flowerTracker.gardenSize;

            // Garden area thresholds are fixed for each stage, and does not scale with number of ideo/precept believers
            if (gardenSize == 0)
            {
                return ThoughtState.ActiveAtStage(0);
            }
            if (gardenSize > 0 && gardenSize < flowerTracker.GardenAreaThresLow)
            {
                return ThoughtState.ActiveAtStage(1);
            }
            return ThoughtState.Inactive;
        }
    }
}
