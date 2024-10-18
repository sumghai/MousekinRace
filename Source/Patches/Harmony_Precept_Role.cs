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
            if (___ideo.culture.IsMousekin() && !__instance.def.leaderRole && __instance.def == PreceptDefOf.IdeoRole_Moralist)
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
            if (___ideo.culture.IsMousekin()) 
            {
                __result = null;
                return false;
            }
            return true;
        }
    }

    // Conditionally patch leader and moral guide role titles for Mousekin player faction
    [HarmonyPatch(typeof(Precept_Role), nameof(Precept_Role.LabelForPawn))]
    public static class Harmony_Precept_Role_LabelForPaw_ReplaceRoleTitlesForMousekinPlayer
    {
        static void Postfix(ref string __result, Pawn p, ref Precept_Role __instance)
        {            
            if (p.Faction.IsPlayer)
            {
                __result = Utils.ReplaceIdeoRoleTitlesForMousekinPlayer(__result, __instance);
            }
        }
    }
}
