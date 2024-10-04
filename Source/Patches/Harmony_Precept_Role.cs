using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse.Grammar;
using Verse;

namespace MousekinRace
{
    // Replace moralist role precept namemaker for Mousekin ideos with a custom, simplified one
    // (which excludes adjectives, dieties, ideo names as descriptors from the role name)
    [HarmonyPatch(typeof(Precept_Role), nameof(Precept_Role.GenerateNameRaw))]
    public static class Harmony_Precept_Role_UseSimplfiedNamemakerForMoralist
    {
        static bool Prefix(Precept_Role __instance, ref string __result, Ideo ___ideo)
        {
            if (___ideo.culture.defName.Contains("Mousekin") && !__instance.def.leaderRole && __instance.def == PreceptDefOf.IdeoRole_Moralist)
            {
                GrammarRequest request = default;
                request.Includes.Add(MousekinDefOf.NamerRoleMoralist_Mousekin);
                __instance.AddIdeoRulesTo(ref request);
                __result = GenText.CapitalizeAsTitle(GrammarResolver.Resolve("r_roleName", request, null, forceLog: false, null, null, null, capitalizeFirstSentence: false));
                return false;
            }
            return true;
        }
    }
    
    // Disable role precept apparel requirements in Mousekin ideos/cultures
    [HarmonyPatch(typeof(Precept_Role), nameof(Precept_Role.GenerateNewApparelRequirements))]
    public static class Harmony_Precept_Role_DisableApparelRequirementsForMousekinIdeos
    {
        static bool Prefix(ref List<PreceptApparelRequirement> __result, Ideo ___ideo)
        {
            if (___ideo.culture.defName.Contains("Mousekin")) 
            {
                __result = null;
                return false;
            }
            return true;
        }
    }
}
