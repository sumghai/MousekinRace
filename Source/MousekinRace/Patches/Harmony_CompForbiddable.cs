using HarmonyLib;
using RimWorld;

namespace MousekinRace
{
    // Skip drawing the forbidden overlay for items stored inside Root Cellars or storage buildings with the CompStorageHiddenContents comp
    [HarmonyPatch(typeof(CompForbiddable), nameof(CompForbiddable.UpdateOverlayHandle))]
    public static class Harmony_CompForbiddable_UpdateOverlayHandle_SkipDrawingForRootCellarContents
    {
        static bool Prefix(CompForbiddable __instance)
        {
            return !(__instance.parent.Spawned && (Building_CellarOutdoor.ThingIsInCellar(__instance.parent) || CompStorageHiddenContents.ThingIsInStorage(__instance.parent)));
        }
    }
}
