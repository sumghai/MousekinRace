using RimWorld;
using Verse;
using Verse.AI;

namespace MousekinRace
{
    public class MentalStateWorker_EarlessSuicide : MentalStateWorker
    {
		// Earless Mousekins will only attempt suicide in the privacy of their own bedroom or barrack
		public override bool StateCanOccur(Pawn pawn)
		{
			if (!base.StateCanOccur(pawn))
			{
				return false;
			}
			Building_Bed ownedBed = pawn.ownership.OwnedBed;
			if (ownedBed == null || ownedBed.GetRoom() == null || ownedBed.GetRoom().PsychologicallyOutdoors)
			{
				return false;
			}
			return true;
		}
	}
}
