using HarmonyLib;
using RimWorld;
using Verse;

namespace MousekinRace
{
    // Conditionally patch leader and moral guide role titles for Mousekin player faction
    // in the ideo conversion power statpart explanation display
    [HarmonyPatch(typeof(StatPart_RoleConversionPower), nameof(StatPart_RoleConversionPower.ExplanationPart))]
    public static class Harmony_StatPart_RoleConversionPower_ExplanationPart_ReplaceRoleTitlesForMousekinPlayer
    {
        static void Postfix(ref string __result, StatRequest req)
        {
            if (__result != null && req.Thing is Pawn pawn && pawn.Ideo.GetRole(pawn) is Precept_Role role) 
            {
                __result = Utils.ReplaceIdeoRoleTitlesForMousekinPlayer(__result, role);
            }
        }
    }
}
