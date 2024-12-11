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

            if (gardenSize == 0)
            {
                return ThoughtState.ActiveAtStage(0);
            }
            if (gardenSize > 0 && gardenSize < 32)
            {
                return ThoughtState.ActiveAtStage(1);
            }
            return ThoughtState.Inactive;
        }
    }
}
