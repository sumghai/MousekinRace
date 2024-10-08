﻿using Verse;

namespace MousekinRace
{
    public class CompProperties_ApparelWithAltStateGraphics : CompProperties
    {
        [NoTranslate]
        public string altStateWornGraphicPath;

        public string toggleLabel = "Toggle apparel"; // Placeholder defaults

        public string toggleDesc = "Toggle this apparel's state"; // Placeholder defaults

        public CompProperties_ApparelWithAltStateGraphics()
        {
            compClass = typeof(CompApparelWithAltStateGraphics);
        }
    }
}
