using HarmonyLib;
using RimWorld;

namespace MousekinRace
{
    // Conditionally patch leader and moral guide titles for Mousekin player faction
    // for pawn ritual role selection widget tooltip text
    [HarmonyPatch(typeof(PawnRitualRoleSelectionWidget), nameof(PawnRitualRoleSelectionWidget.ExtraRoleInfo))]
    public static class Harmony_PawnRitualRoleSelectionWidget_ExtraRoleInfo_ReplaceRoleTitlesForMousekinPlayer
    {
        static void Postfix(ref string __result, RitualRole role, Precept_Ritual ___ritual)
        {
            if (__result != null) 
            {
                __result = Utils.ReplaceIdeoRoleTitlesForMousekinPlayer(__result, role.FindInstance(___ritual.ideo));
            }
        }
    }
}
