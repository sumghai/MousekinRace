using HarmonyLib;
using RimWorld;

namespace MousekinRace
{
    // Update a cached boolean in the Faction Allegiance system every time a pawn gains or loses a royal title with the Shattered Empire
    [HarmonyPatch(typeof(Pawn_RoyaltyTracker), nameof(Pawn_RoyaltyTracker.OnPostTitleChanged))]
    public static class Harmony_Pawn_RoyaltyTracker_OnPostTitleChanged_CheckForAnyPawnsWithEmpireTitle
    {
        static void Postfix()
        {
            GameComponent_Allegiance.Instance.anyColonistsWithShatteredEmpireTitle = AllegianceSys_Utils.AnyColonistsWithShatteredEmpireTitle();
        }
    }
}
