using RimWorld;
using Verse;

namespace MousekinRace
{
    public class ThoughtWorker_OtherMousekinHasProstheticOpinions : ThoughtWorker
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

            if (pawnFaithTraitDegree < 2)
            {
                return false;
            }

            int numOfProstheticsOnOtherPawn = otherPawn.health.hediffSet.CountAddedAndImplantedParts();

            if (numOfProstheticsOnOtherPawn < 1)
            {
                return false;
            }

            return true;
        }
    }
}
