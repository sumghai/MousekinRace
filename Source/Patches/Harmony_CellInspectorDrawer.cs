using HarmonyLib;
using Verse;

namespace MousekinRace
{
    // Skip listing items stored inside Root Cellars or storage buildings with the CompStorageHiddenContents comp
    // in the Cell Inspector context popup (Mouse over cell while holding Alt key)
    [HarmonyPatch(typeof(CellInspectorDrawer), nameof(CellInspectorDrawer.DrawThingRow))]
    public static class Harmony_CellInspectorDrawer_DrawThingRow_RootCellarsSkipListingContents
    {
        static bool Prefix(Thing thing)
        {
            return !(Building_CellarOutdoor.ThingIsInCellar(thing) || CompStorageHiddenContents.ThingIsInStorage(thing));
        }
    }
}
