using HarmonyLib;
using RimWorld;
using Verse;

namespace MousekinRace
{
    // Conditionally patch leader and moral guide role titles for Mousekin player faction
    // in the ideo conversion ability mouseover text
    [HarmonyPatch(typeof(CompAbilityEffect_Convert), nameof(CompAbilityEffect_Convert.ExtraLabelMouseAttachment))]
    public static class Harmony_CompAbilityEffect_Convert_ExtraLabelMouseAttachment_ReplaceRoleTitlesForMousekinPlayer
    {
        static void Postfix(ref string __result, LocalTargetInfo target)
        {
            if (__result != null) 
            {
                Pawn targetPawn = target.Pawn;
                Precept_Role role = targetPawn.Ideo?.GetRole(targetPawn);
                __result = Utils.ReplaceIdeoRoleTitlesForMousekinPlayer(__result, role);
            }
        }
    }
}
