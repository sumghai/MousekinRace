using UnityEngine;
using Verse;

namespace MousekinRace
{
    public class Settings : ModSettings
    {
        public bool EarlessMousekinsAreSuicidal = true;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref EarlessMousekinsAreSuicidal, "EarlessMousekinsAreSuicidal", true, true);
        }

        public void Draw(Rect canvas)
        {
            var listingStandard = new Listing_Standard();
            listingStandard.Begin(canvas);

            listingStandard.CheckboxLabeled("MousekinRace_Settings_EarlessMousekinsAreSuicidal_Title".Translate(),
                ref EarlessMousekinsAreSuicidal, "MousekinRace_Settings_EarlessMousekinsAreSuicidal_Desc".Translate());

            listingStandard.End();
        }
    }
}
