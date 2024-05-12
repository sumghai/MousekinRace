using System.Collections.Generic;
using Verse;

namespace MousekinRace
{
    public class CompProperties_ApparelWithAttachedHeadgear : CompProperties
    {
        [NoTranslate]
        public string attachedHeadgearGraphicPath;

        [NoTranslate]
        public string toggleUiIconPath;

        [MustTranslate]
        public string attachedHeadgearLabel;

        [NoTranslate]
        public List<string> hideHarBodyAddonsWithTag = new();

        public CompProperties_ApparelWithAttachedHeadgear()
        {
            compClass = typeof(CompApparelWithAttachedHeadgear);
        }
    }
}
