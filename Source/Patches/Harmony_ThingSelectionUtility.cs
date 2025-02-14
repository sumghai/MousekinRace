using HarmonyLib;
using RimWorld;
using Verse;

namespace MousekinRace
{
    [HarmonyPatch(typeof(ThingSelectionUtility), nameof(ThingSelectionUtility.SelectableByMapClick))]
    public static class ThingSelectionUtility_SelectableByMapClick_PreventSelectionOfRootCellarContents
    {
        static void Postfix(ref bool __result, Thing t)
        {
            if (Building_CellarOutdoor.ThingIsInCellar(t))
            {
                __result = false;
            }
        }
    }
}
