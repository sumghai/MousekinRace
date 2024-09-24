using HarmonyLib;
using RimWorld;
using System.Collections.Generic;

namespace MousekinRace
{
    // Disable role precept apparel requirements in Mousekin ideos
    [HarmonyPatch(typeof(Precept_Role), nameof(Precept_Role.GenerateNewApparelRequirements))]
    public static class Harmony_Precept_Role
    {
        static bool Prefix(ref List<PreceptApparelRequirement> __result, FactionDef generatingFor)
        {
            if (generatingFor.categoryTag != null && generatingFor.categoryTag.Contains("Mousekin")) 
            {
                __result = null;
                return false;
            }
            return true;
        }
    }
}
