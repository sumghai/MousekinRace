using RimWorld;
using Verse;

namespace MousekinRace
{
    public class ThoughtWorker_PiousVsAtheist : ThoughtWorker
    {
        public override ThoughtState CurrentSocialStateInternal(Pawn pawn, Pawn otherPawn)
        {
            // Mousekins are always Humanlike, so we can skip to directly checking if both pawns are Mousekins
            if (!Utils.IsMousekin(pawn) || !Utils.IsMousekin(otherPawn))
            {
                return false;
            }
            if (!pawn.story.traits.HasTrait(MousekinDefOf.Mousekin_Trait_Pious))
            {
                return false;
            }
            if (!otherPawn.story.traits.HasTrait(MousekinDefOf.Mousekin_Trait_Atheist))
            {
                return false;
            }
            if (!RelationsUtility.PawnsKnowEachOther(pawn, otherPawn))
            {
                return false;
            }
            return true;
        }
    }
}
