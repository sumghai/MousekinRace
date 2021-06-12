using HarmonyLib;
using RimWorld;
using Verse;

namespace MousekinRace.Patches
{
    [HarmonyPatch(typeof(PawnBioAndNameGenerator), nameof(PawnBioAndNameGenerator.FillBackstorySlotShuffled))]
    public static class Temp_Harmony_PawnBioAndNameGenerator
    {
        static void Prefix(Pawn pawn, BackstorySlot slot)
        {
            //Log.Warning("Generating " + slot + " bio for " + pawn + " (" + pawn.ageTracker.AgeBiologicalYears + ", " + pawn.gender + ", " + pawn.kindDef + ") of " + pawn.Faction + " (" + pawn.Faction.def + ")");
            //Log.Warning("---------");
        }
    }
}
