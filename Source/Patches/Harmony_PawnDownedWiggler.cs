using HarmonyLib;
using Verse;

namespace MousekinRace
{
    // Don't wriggle downed heretics being executed in the Purging Flames ritual
    [HarmonyPatch(typeof(PawnDownedWiggler), nameof(PawnDownedWiggler.ProcessPostTickVisuals))]
    public static class Harmony_PawnDownedWiggler_ProcessPostTickVisuals_SkipForHereticsBeingExecuted
    {
        static bool Prefix(PawnDownedWiggler __instance)
        { 
            Pawn pawn = __instance.pawn;
            if (pawn.IsHereticBeingPurgedByFire()) 
            {
                __instance.downedAngle = 0;
                return false;
            }
            return true;
        }
    }
}
