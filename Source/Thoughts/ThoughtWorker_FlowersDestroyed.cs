using RimWorld;
using Verse;

namespace MousekinRace
{
	public class ThoughtWorker_FlowersDestroyed : ThoughtWorker_Precept
	{
		public override ThoughtState ShouldHaveThought(Pawn p)
		{
			MapComponent_FlowerTracker flowerTracker = p.Map.GetComponent<MapComponent_FlowerTracker>();
			int flowersDestroyed = flowerTracker.playerFlowerDestructionTicks.Count;

			// Number of flowers destroyed is fixed for each stage
			// (i.e. does not scale with number of believers in the precept)
			if (flowersDestroyed > 0 && flowersDestroyed < 5)
			{
				return ThoughtState.ActiveAtStage(0);
			}
			if (flowersDestroyed >= 5 && flowersDestroyed < 10)
			{
				return ThoughtState.ActiveAtStage(1);
			}
			if (flowersDestroyed >= 10 && flowersDestroyed < 20)
			{
				return ThoughtState.ActiveAtStage(2);
			}
			if (flowersDestroyed >= 20)
			{
				return ThoughtState.ActiveAtStage(3);
			}
			return ThoughtState.Inactive;
		}
	}
}