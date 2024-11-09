using RimWorld;
using Verse;

namespace MousekinRace
{
    public class Alert_FactionAllegianceAvailable : Alert
    {
        public Alert_FactionAllegianceAvailable()
        {           
            defaultLabel = "AlertFactionAllegianceAvailableLabel".Translate();
            defaultPriority = AlertPriority.High;
        }

        public override TaggedString GetExplanation()
        {
            return "AlertFactionAllegianceAvailableDesc".Translate();
        }

        public override void OnClick()
        {
            Find.MainTabsRoot.SetCurrentTab(MousekinDefOf.Mousekin_MainButton_Allegiance);
        }

        public override AlertReport GetReport()
        {
            GameComponent_Allegiance allegianceGameComp = GameComponent_Allegiance.Instance;

            return allegianceGameComp.HasAnyTownSquares && !allegianceGameComp.HasDeclaredAllegiance && allegianceGameComp.anyAllegianceFactionJoinReqsMet;
        }
    }
}
