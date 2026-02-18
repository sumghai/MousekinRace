using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.Grammar;

namespace GrimalkinRace
{
    // Replace moralist role precept namemaker for Grimalkin ideos with a custom, simplified one
    // (which excludes adjectives, dieties, ideo names as descriptors from the role name)
    [HarmonyPatch(typeof(Precept_Role), nameof(Precept_Role.GenerateNameRaw))]
    public static class Harmony_Precept_Role_UseSimplfiedNamemakerForMoralist
    {
        static bool Prefix(Precept_Role __instance, ref string __result, Ideo ___ideo)
        {
            if (___ideo.culture.IsGrimalkin() && !__instance.def.leaderRole && __instance.def == PreceptDefOf.IdeoRole_Moralist)
            {
                GrammarRequest request = default;
                request.Includes.Add(GrimalkinDefOf.NamerRoleMoralist_Grimalkin);
                __instance.AddIdeoRulesTo(ref request);
                __result = GenText.CapitalizeAsTitle(GrammarResolver.Resolve("r_roleName", request, null, forceLog: false, null, null, null, capitalizeFirstSentence: false));
                return false;
            }
            return true;
        }
    }

    // Remove role precept apparel requirements in Grimalkin ideos/cultures
    [HarmonyPatch(typeof(Precept_Role), nameof(Precept_Role.GenerateNewApparelRequirements))]
    public static class Harmony_Precept_Role_OverrideApparelRequirementsForGrimalkinIdeos
    {
        static bool Prefix(ref List<PreceptApparelRequirement> __result, Ideo ___ideo)
        {
            if (___ideo.culture.IsGrimalkin())
            {
                // Clear existing apparel requirements
                __result = null;
                return false;
            }
            return true;
        }
    }
}
