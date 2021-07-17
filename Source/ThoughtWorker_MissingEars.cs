using RimWorld;
using System.Linq;
using Verse;

namespace MousekinRace
{
    public class ThoughtWorker_MissingEars : ThoughtWorker
    {
        public override ThoughtState CurrentStateInternal(Pawn pawn)
        {
            // Mousekins are always Humanlike, so we can skip to directly checking if both pawns are Mousekins
            if (!Utils.IsMousekin(pawn))
            {
                return ThoughtState.Inactive;
            }

            int numOfMissingEars = pawn.health.hediffSet.cachedMissingPartsCommonAncestors.Count(x => x.Part.def == MousekinDefOf.Mousekin_Ear);

            int numOfProstheticEars = pawn.health.hediffSet.GetHediffCount(MousekinDefOf.Mousekin_ProstheticClothEar);

            if (numOfMissingEars == 0 && numOfProstheticEars == 1)
            {
                return ThoughtState.ActiveAtStage(0);
            }

            if (numOfMissingEars == 0 && numOfProstheticEars == 2)
            {
                return ThoughtState.ActiveAtStage(1);
            }

            if (numOfMissingEars == 1 && numOfProstheticEars == 0)
            {
                return ThoughtState.ActiveAtStage(2);
            }

            if (numOfMissingEars == 1 && numOfProstheticEars == 1)
            {
                return ThoughtState.ActiveAtStage(3);
            }

            if (numOfMissingEars == 2 && numOfProstheticEars == 0)
            {
                return ThoughtState.ActiveAtStage(4);
            }

            return ThoughtState.Inactive;
        }
    }
}
