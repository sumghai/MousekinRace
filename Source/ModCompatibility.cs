using Verse;

namespace MousekinRace
{
    [StaticConstructorOnStartup]
    public class ModCompatibility
    {
        public static bool VanillaCookingExpandedIsActive => ModsConfig.IsActive("VanillaExpanded.VCookE");

        public static bool MedievalOverhaulIsActive => ModsConfig.IsActive("DankPyon.Medieval.Overhaul");
    }
}
