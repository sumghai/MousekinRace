using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI.Group;

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

    // For Leader speech and Role change rituals, if the ritual target is a Town Square,
    // override the "face spectator" target cell to the center cell of the square
    [HarmonyPatch(typeof(RitualUtility), nameof(RitualUtility.RitualCrowdCenterFor))]
    public static class Harmony_RitualUtility_RitualCrowdCenterFor_OverrideForTownSquare
    {
        static bool Prefix(Pawn pawn, ref IntVec3 __result)
        {
            if (pawn.GetLord()?.LordJob is LordJob_Ritual lordJob_Ritual && pawn.CurJob != null && pawn.CurJob.speechFaceSpectatorsIfPossible && lordJob_Ritual.selectedTarget.Thing is Building_TownSquare townSquare)
            {
                __result = townSquare.centerCellPos;
                return false;
            }
            return true;
        }
    }
}
