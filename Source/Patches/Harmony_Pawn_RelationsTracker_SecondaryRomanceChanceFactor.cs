using HarmonyLib;
using RimWorld;
using Verse;

namespace MousekinRace
{
    // Prevent specially marked pawnkinds from ever starting new relationships
    [HarmonyPatch(typeof(Pawn_RelationsTracker), nameof(Pawn_RelationsTracker.SecondaryRomanceChanceFactor))]
    public static class Harmony_Pawn_RelationsTracker_SecondaryRomanceChanceFactor_PreventNewRomancesForRestrictedPawnKinds
    {
        public static void Postfix(Pawn otherPawn, Pawn ___pawn, ref float __result)
        {
            if ((___pawn.kindDef.GetModExtension<RomanceUtilsExtension>()?.preventNewRomances == true) || (otherPawn.kindDef.GetModExtension<RomanceUtilsExtension>()?.preventNewRomances == true))
            {
                __result = 0f;
            }
        }
    }
}
