using Verse;

namespace MousekinRace
{
    [StaticConstructorOnStartup]
    public class ModCompatibility
    {
        public static bool VanillaCookingExpandedIsActive => ModsConfig.IsActive("VanillaExpanded.VCookE");
    }
}
