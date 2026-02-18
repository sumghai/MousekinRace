using HarmonyLib;
using RimWorld;
using Verse;

namespace MousekinRace
{
    // Prevent selection of items stored inside Root Cellars or storage buildings with the CompStorageHiddenContents comp
    [HarmonyPatch(typeof(ThingSelectionUtility), nameof(ThingSelectionUtility.SelectableByMapClick))]
    public static class Harmony_ThingSelectionUtility_SelectableByMapClick_PreventSelectionOfRootCellarContents
    {
        static void Postfix(ref bool __result, Thing t)
        {
            if (Building_CellarOutdoor.ThingIsInCellar(t) || CompStorageHiddenContents.ThingIsInStorage(t))
            {
                __result = false;
            }
        }
    }
}
