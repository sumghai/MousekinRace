using RimWorld;
using Verse.AI;

namespace MousekinRace
{
    public class MentalState_ExitAfterAllegianceChange : MentalState
    {
        public override RandomSocialMode SocialModeMax()
        {
            return RandomSocialMode.Off;
        }

        public override void PostEnd()
        {
            base.PostEnd();
            AllegianceSys_Utils.SetFactionToOpposingAllegiance(pawn);
        }
    }
}
