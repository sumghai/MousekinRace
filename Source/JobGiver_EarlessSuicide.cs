using Verse;
using Verse.AI;

namespace MousekinRace
{
    public class JobGiver_EarlessSuicide : JobGiver_Wander
    {
		public JobGiver_EarlessSuicide()
		{
			wanderRadius = 7f;
			ticksBetweenWandersRange = new IntRange(125, 200);
			locomotionUrgency = LocomotionUrgency.Amble;
			wanderDestValidator = ((Pawn pawn, IntVec3 loc, IntVec3 root) => WanderRoomUtility.IsValidWanderDest(pawn, loc, root));
		}

		public override IntVec3 GetWanderRoot(Pawn pawn)
		{
			return pawn.Position;
		}
	}
}
