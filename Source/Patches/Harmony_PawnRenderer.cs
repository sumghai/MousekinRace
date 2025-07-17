using HarmonyLib;
using Verse;

namespace MousekinRace
{
    // Keep downed heretics being executed in the Purging Flames ritual facing forwards
    [HarmonyPatch(typeof(PawnRenderer), nameof(PawnRenderer.LayingFacing))]
    public static class PawnRenderer_LayingFacing_FaceForwardForHereticsBeingExecuted
    {
        public static void Postfix(ref Rot4 __result, Pawn ___pawn)
        {
            if (___pawn.Downed && ___pawn.IsHereticBeingPurgedByFire())
            {
                __result = Rot4.South;
            }
        }
    }

    // Hide pawns working inside the Mine Entrance
    [HarmonyPatch(typeof(PawnRenderer), nameof(PawnRenderer.RenderPawnAt))]
    public static class PawnRenderer_RenderPawnAt_HidePawnsWorkingInsideMineEntrance
    {
        public static bool Prefix(Pawn ___pawn)
        {
            if (___pawn?.Map != null && ___pawn.Spawned && ___pawn.jobs?.curDriver?.CurToilString == "Mousekin_Toil_MineResources")
            {
                return false;
            }
            return true;
        }
    }
}
