using HarmonyLib;
using RimWorld;

namespace MousekinRace
{
    // Pawns arrested while experiencing the "Leaving colony due to allegiance change" mental state
    // should change their faction to the enemy of the player-chosen allegiance faction
    [HarmonyPatch(typeof(Pawn_GuestTracker), nameof(Pawn_GuestTracker.CapturedBy))]
    public static class Harmony_Pawn_GuestTracker_CapturedBy_DissidentPawnsChangeFactionIfAllegianceSet
    {
        static void Prefix(Pawn_GuestTracker __instance)
        {
            if (__instance.pawn.IsColonist && __instance.pawn.MentalStateDef == MousekinDefOf.Mousekin_MentalState_ExitAfterAllegianceChange)
            {
                //AllegianceSys_Utils.SetFactionToOpposingAllegiance(__instance.pawn);
            }
        }
    }
}
