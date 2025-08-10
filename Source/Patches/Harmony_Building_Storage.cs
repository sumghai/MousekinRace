using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MousekinRace
{
    // If a storage building has a custom CompStorageHiddenContents comp,
    // hide individual item selection selection gizmos
    [HarmonyPatch(typeof(Building_Storage), nameof(Building_Storage.GetGizmos))]
    public static class Harmony_Building_Storage_GetGizmos_ConditionallyHideContents
    {
        static void Postfix(Building_Storage __instance, ref IEnumerable<Gizmo> __result)
        {
            if (__instance.def.HasComp(typeof(CompStorageHiddenContents)))
            {
                __result = PatchGetGizmos(__result);
            }
        }

        static IEnumerable<Gizmo> PatchGetGizmos(IEnumerable<Gizmo> __result)
        {
            foreach (Gizmo gizmo in __result) 
            {
                if (gizmo is Command_SelectStorage)
                {
                    continue;
                }
                yield return gizmo;
            }
        }
    }
}
