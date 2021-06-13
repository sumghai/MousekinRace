using RimWorld;
using Verse;

namespace MousekinRace
{
    public class ThoughtWorker_ReligiousDisgustAtOwnArtificalBodyParts : ThoughtWorker
    {
        public override ThoughtState CurrentStateInternal(Pawn p)
        {
            int numOfProsthetics = p.health.hediffSet.CountAddedAndImplantedParts();

            int pawnFaithTraitDegree = p.story.traits.DegreeOfTrait(MousekinDefOf.Mousekin_TraitSpectrum_Faith);

            // Devotionists (2) and Pious (3)
            if (pawnFaithTraitDegree > 1 && pawnFaithTraitDegree < 4)
            {
                if (numOfProsthetics == 1)
                {
                    return ThoughtState.ActiveAtStage(0, numOfProsthetics.ToString());
                }
                if (numOfProsthetics > 1 && numOfProsthetics < 4)
                {
                    return ThoughtState.ActiveAtStage(1, numOfProsthetics.ToString());
                }
                if (numOfProsthetics >= 4)
                {
                    return ThoughtState.ActiveAtStage(2, numOfProsthetics.ToString());
                }
            }

            // Inquisitionists (4)
            if (pawnFaithTraitDegree == 4)
            {
                if (numOfProsthetics == 1)
                {
                    return ThoughtState.ActiveAtStage(3, numOfProsthetics.ToString());
                }
                if (numOfProsthetics > 1 && numOfProsthetics < 4)
                {
                    return ThoughtState.ActiveAtStage(4, numOfProsthetics.ToString());
                }
                if (numOfProsthetics >= 4)
                {
                    return ThoughtState.ActiveAtStage(5, numOfProsthetics.ToString());
                }
            }

            return false;
        }
    }
}
