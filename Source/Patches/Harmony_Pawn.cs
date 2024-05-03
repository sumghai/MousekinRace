using HarmonyLib;
using Verse;

namespace MousekinRace
{
    // Pawns leaving the map while experiencing the "Leaving colony due to allegiance change" mental state
    // should change their faction to the enemy of the player-chosen allegiance faction
    [HarmonyPatch(typeof(Pawn), nameof(Pawn.ExitMap))]
    public static class Harmony_Pawn_ExitMap_DissidentPawnsChangeFactionIfAllegianceSet
    {
        static void Prefix(Pawn __instance)
        { 
            if (__instance.IsColonist && __instance.MentalStateDef == MousekinDefOf.Mousekin_MentalState_ExitAfterAllegianceChange) 
            {
                AllegianceSys_Utils.SetFactionToOpposingAllegiance(__instance);
            }
        }
    }
}
