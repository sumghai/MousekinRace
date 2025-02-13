using HarmonyLib;
using RimWorld;
using Verse;

namespace MousekinRace
{
    // Prevent heretics being executed in the Purging Flames ritual from acting according to the BurningResponse thinktree
    [HarmonyPatch(typeof(ThinkNode_ConditionalBurning), nameof(ThinkNode_ConditionalBurning.Satisfied))]
    public static class Harmony_ThinkNode_ConditionalBurning_Satisfied_SkipForHereticsBeingExecuted
    {        
        static bool Prefix(Pawn pawn)
        {
            if (pawn.IsHereticBeingPurgedByFire())
            {
                return false;
            }
            return true;
        }
    }
}
