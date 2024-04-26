using HarmonyLib;
using RimWorld;
using Verse;

namespace MousekinRace
{
    // Non-player owned Giant Cavies should not spawn pellets
    [HarmonyPatch(typeof(CompSpawner), nameof(CompSpawner.CheckShouldSpawn))]
    public static class Harmony_CompSpawner_CheckShouldSpawn
    {
        [HarmonyPrefix]
        static bool Prefix(CompSpawner __instance) 
        {
            if (__instance.parent is Pawn pawn && pawn.def == MousekinDefOf.Mousekin_AnimalGiantCavy && pawn.Faction != Faction.OfPlayer)
            {
                return false;
            }
            return true;
        }
    }
}
