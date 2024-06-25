using RimWorld;
using Verse;

namespace MousekinRace
{
    public class ThoughtWorker_PrejudicedTowardsMousekins : ThoughtWorker
    {
        public override ThoughtState CurrentSocialStateInternal(Pawn pawn, Pawn otherPawn)
        {
            // Skip if pawn is non-humanlike
            if (!pawn.RaceProps.Humanlike)
            {
                return false;
            }

            // Skip if pawn has kind or HAR's xenophile (degree -1) trait
            if (pawn.story.traits.HasTrait(TraitDefOf.Kind) || pawn.story.traits.HasTrait(AlienRace.AlienDefOf.HAR_Xenophobia, -1))
            {
                return false;
            }

            // Skip if pawn is a Mousekin themself
            if (Utils.IsMousekin(pawn))
            {
                return false;
            }

            // Skip if other pawn is non-humanlike
            if (!otherPawn.RaceProps.Humanlike)
            {
                return false;
            }

            // Skip if other pawn is not a Mousekin
            if (!Utils.IsMousekin(otherPawn))
            {
                return false;
            }

            // Ideology DLC check
            if (ModsConfig.IdeologyActive)
            {
                // Skip if pawn's ideo has HAR Alien Race general relation Respected or Exhalted precepts
                if (pawn.ideo.ideo.HasPrecept(MousekinDefOf.HAR_AlienRaces_Respected) || pawn.ideo.ideo.HasPrecept(MousekinDefOf.HAR_AlienRaces_Exalted))
                {
                    return false;
                }

                // Skip if pawn's ideo has Animal Personhood meme
                if (pawn.ideo.ideo.HasMeme(MousekinDefOf.AnimalPersonhood))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
