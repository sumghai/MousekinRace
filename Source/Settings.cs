using UnityEngine;
using Verse;

namespace MousekinRace
{
    public class Settings : ModSettings
    {
        public bool EarlessMousekinsAreSuicidal = true;
        public int AllegianceSys_DaysBetweenRandomTraders = 5;

        public override void ExposeData()
        {
            base.ExposeData();
            var defaults = new Settings();
            Scribe_Values.Look(ref EarlessMousekinsAreSuicidal, "EarlessMousekinsAreSuicidal", defaults.EarlessMousekinsAreSuicidal, true);
            Scribe_Values.Look(ref AllegianceSys_DaysBetweenRandomTraders, "AllegianceSys_DaysBetweenRandomTraders", defaults.AllegianceSys_DaysBetweenRandomTraders, true);
        }

        public void Draw(Rect canvas)
        {           
            var listingStandard = new Listing_Standard()
            { 
                ColumnWidth = (canvas.width - Window.StandardMargin) / 2
            };

            listingStandard.Begin(canvas);

            // First Column
            listingStandard.Gap();
            listingStandard.Header("MousekinRace_Settings_SectionRace_Heading".Translate());
            listingStandard.CheckboxLabeled("MousekinRace_Settings_SectionRace_EarlessMousekinsAreSuicidal_Label".Translate(),
                ref EarlessMousekinsAreSuicidal, "MousekinRace_Settings_SectionRace_EarlessMousekinsAreSuicidal_Tooltip".Translate());

            // Second Column
            listingStandard.NewColumn();
            listingStandard.Gap();

            listingStandard.Header("MousekinRace_Settings_SectionAllegianceSys_Heading".Translate());
            AllegianceSys_DaysBetweenRandomTraders = (int) listingStandard.SliderLabeled("MousekinRace_Settings_SectionAllegianceSys_RandomTradeCaravanInterval_Label".Translate("PeriodDays".Translate(AllegianceSys_DaysBetweenRandomTraders)), AllegianceSys_DaysBetweenRandomTraders, 3, 10, 0.65f, "MousekinRace_Settings_SectionAllegianceSys_RandomTradeCaravanInterval_Tooltip".Translate());

            listingStandard.End();
        }
    }
}
