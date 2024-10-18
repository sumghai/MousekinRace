using HarmonyLib;
using RimWorld;

namespace MousekinRace
{
    // Conditionally patch leader and moral guide role titles for Mousekin player faction
    // in the extra confirmation text of the role change confirmation dialog
    [HarmonyPatch(typeof(RitualUtility), nameof(RitualUtility.RoleChangeConfirmation))]
    public static class Harmony_RitualUtility_RoleChangeConfirmation_ReplaceRoleTitlesForMousekinPlayer
    {
        static void Postfix(ref string __result, Precept_Role oldRole, Precept_Role newRole)
        {
            __result = Utils.ReplaceIdeoRoleTitlesForMousekinPlayer(__result, oldRole);
            __result = Utils.ReplaceIdeoRoleTitlesForMousekinPlayer(__result, newRole);
        }
    }
}
