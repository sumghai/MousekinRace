using RimWorld;

namespace MousekinRace
{
    public class Thought_OtherMousekinHasProstheticOpinions : Thought_SituationalSocial
    {
		public override float OpinionOffset()
		{
			if (ThoughtUtility.ThoughtNullified(pawn, def))
			{
				return 0f;
			}

			// Degrees of Mousekin_TraitSpectrum_Faith:
			//
			// 0 = None / undefined
			// 1 = Apostate
			// 2 = Devotionist
			// 3 = Pious
			// 4 = Inquisitionist

			int pawnFaithTraitDegree = pawn.story.traits.DegreeOfTrait(MousekinDefOf.Mousekin_TraitSpectrum_Faith);

			// Nones and Apostates
			if (pawnFaithTraitDegree <= 1)
			{
				return 0f;
			}
			// Devotionists
			if (pawnFaithTraitDegree == 2)
			{
				return -5f;
			}
			// Pious
			if (pawnFaithTraitDegree == 3)
			{
				return -15f;
			}
			// Inquisitionists
			return -20f;
		}
	}
}
