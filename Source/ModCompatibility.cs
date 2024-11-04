using Verse;

namespace MousekinRace
{
    [StaticConstructorOnStartup]
    public class ModCompatibility
    {
        public static bool VanillaCookingExpandedIsActive => ModsConfig.IsActive("VanillaExpanded.VCookE");

        public static bool MedievalOverhaulIsActive => ModsConfig.IsActive("DankPyon.Medieval.Overhaul");

        public static bool LwmDeepStorageIsActive => ModsConfig.IsActive("LWM.DeepStorage");

        public static bool OrionHospitalityIsActive => ModsConfig.IsActive("Orion.Hospitality");
    }
}
