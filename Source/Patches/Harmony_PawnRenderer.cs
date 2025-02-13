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
}
