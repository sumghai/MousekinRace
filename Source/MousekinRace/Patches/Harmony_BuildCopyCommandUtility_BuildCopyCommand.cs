using HarmonyLib;
using RimWorld;
using Verse;

namespace MousekinRace
{
    // Disable Build Copy command for buildings that are limited to one per map (e.g. Town Square)
    [HarmonyPatch(typeof(BuildCopyCommandUtility), nameof(BuildCopyCommandUtility.BuildCopyCommand))]
    public static class Harmony_BuildCopyCommandUtility_BuildCopyCommand
    {
        static bool Prefix(BuildableDef buildable)
        {
            return !buildable.placeWorkers?.Contains(typeof(PlaceWorker_OnlyOnePerMap)) ?? true;
        }
    }
}
