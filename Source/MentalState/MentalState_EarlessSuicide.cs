using RimWorld;
using Verse;
using Verse.AI;

namespace MousekinRace
{
    public class MentalState_EarlessSuicide : MentalState
    {
		public IntVec3 target;

		public override void PostStart(string reason)
		{
			base.PostStart(reason);
			if (pawn.ownership.OwnedBed != null)
			{
				target = pawn.ownership.OwnedBed.Position;
			}
			else
			{
				target = pawn.Position;
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref target, "target");
		}

		public override RandomSocialMode SocialModeMax()
		{
			return RandomSocialMode.Off;
		}
	}
}
