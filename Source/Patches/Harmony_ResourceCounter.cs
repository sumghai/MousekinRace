using HarmonyLib;
using RimWorld;
using System.Linq;
using Verse;

namespace MousekinRace
{
    // Include contents of Root Cellars when counting items in the Resource Readout on the top-left corner of the UI
    [HarmonyPatch(typeof(ResourceCounter), nameof(ResourceCounter.UpdateResourceCounts))]
    public static class Harmony_ResourceCounter_UpdateResourceCounts_IncludeItemsInsideRootCellars
    {
        static void Postfix(ResourceCounter __instance)
        {            
            foreach (Building_StorageCellar storageCellar in GameComponent_StorageCellars.Instance.storageCellarsCache.Where(b => b.Map == __instance.map))
            {
                foreach (Thing heldThing in storageCellar.StoredItems)
                {
                    Thing innerIfMinified = heldThing.GetInnerIfMinified();
                    if (innerIfMinified.def.CountAsResource && __instance.ShouldCount(innerIfMinified))
                    {
                        __instance.countedAmounts[innerIfMinified.def] += innerIfMinified.stackCount;
                    }
                }
            }
        }
    }
}
