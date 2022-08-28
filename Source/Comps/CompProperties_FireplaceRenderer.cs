using RimWorld;
using Verse;

namespace MousekinRace
{
    public class CompProperties_FireplaceRenderer : CompProperties_FireOverlay
    {
        public GraphicData fireboxGraphicData = null;

        public GraphicData glowGraphicData = null;

        public CompProperties_FireplaceRenderer()
        {
            compClass = typeof(CompFireplaceRenderer);
        }
    }
}
