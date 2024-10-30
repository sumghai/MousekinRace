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

    // Conditionally append leader and moral guide role titles for Mousekin player faction
    // to the role precept box display in the Ideo tab
    [HarmonyPatch(typeof(Precept_Role), nameof(Precept_Role.UIInfoSecondLine), MethodType.Getter)]
    public static class Harmony_Precept_Role_UIInfoSecondLine_AppendRoleTitlesForMousekinPlayer
    {
        static void Postfix(ref string __result, ref Precept_Role __instance)
        {
            if (Utils.IsMousekin(__instance.ideo.culture) && Faction.OfPlayer.ideos.Has(__instance.ideo))
            {
                // Leader titles should only be appended if
                // - the player's allegiance faction ideo/culture matches the current role's ideo/culture
                // - the player has no allegiance, but the current role's ideo/culture is from the Kingdom
                bool shouldAppendPlayerLeaderTitle = __instance.def.leaderRole && (GameComponent_Allegiance.Instance.alignedFaction?.ideos.PrimaryCulture == __instance.ideo.culture || (!GameComponent_Allegiance.Instance.HasDeclaredAllegiance && Utils.IsMousekinKingdomLike(__instance.ideo.culture)));

                // Moral Guide titles should only be appended if
                // - there are enough believers for the ideo
                // - the player has no allegiance, but the current role's ideo/culture is from the Kingdom
                bool shouldAppendPlayerMoralistTitle = __instance.def == PreceptDefOf.IdeoRole_Moralist && __instance.ideo.colonistBelieverCountCached > __instance.def.deactivationBelieverCount || (!GameComponent_Allegiance.Instance.HasDeclaredAllegiance && Utils.IsMousekinKingdomLike(__instance.ideo.culture));

                if (shouldAppendPlayerLeaderTitle || shouldAppendPlayerMoralistTitle)
                {
                    string originalRoleTitle = __result;
                    string playerVariantRoleTitle = Utils.ReplaceIdeoRoleTitlesForMousekinPlayer(__result, __instance);
                    __result = originalRoleTitle + ((playerVariantRoleTitle != originalRoleTitle) ? " (" + playerVariantRoleTitle + ")" : "");
                }
            }
        }
    }
}
