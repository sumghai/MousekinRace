using RimWorld;
using Verse;

namespace MousekinRace
{
    public class ThoughtWorker_FaithAffinityOpinions : ThoughtWorker
    {
        public override ThoughtState CurrentSocialStateInternal(Pawn pawn, Pawn otherPawn)
        {
            // Mousekins are always Humanlike, so we can skip to directly checking if both pawns are Mousekins
            if (!Utils.IsMousekin(pawn) || !Utils.IsMousekin(otherPawn))
            {
                return false;
            }

            if (!RelationsUtility.PawnsKnowEachOther(pawn, otherPawn))
            {
                return false;
            }

            // Degrees of Mousekin_TraitSpectrum_Faith:
            //
            // 0 = None / undefined
            // 1 = Apostate
            // 2 = Devotionist
            // 3 = Pious
            // 4 = Inquisitionist

            int pawnFaithTraitDegree = pawn.story.traits.DegreeOfTrait(MousekinDefOf.Mousekin_TraitSpectrum_Faith);
            int otherPawnFaithTraitDegree = otherPawn.story.traits.DegreeOfTrait(MousekinDefOf.Mousekin_TraitSpectrum_Faith);

            if (pawnFaithTraitDegree > 0)
            {
                int faithOpinionStage = (pawnFaithTraitDegree - 1) * 5 + otherPawnFaithTraitDegree;
                return ThoughtState.ActiveAtStage(faithOpinionStage);
            }

            return false;
        }
    }
}
