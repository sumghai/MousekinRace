using HarmonyLib;
using RimWorld;
using Verse;

namespace MousekinRace
{
    // Swap the worn graphic path of an apparel depending on the Gizmo state of CompApparelWithAltStateGraphics 
    [HarmonyPatch(typeof(Apparel), nameof(Apparel.WornGraphicPath), MethodType.Getter)]
    public static class Harmony_Apparel_WornGraphicPath_ToggleAltStateGraphics
    {
        public static void Postfix(ref string __result, Apparel __instance)
        { 
            Apparel apparel = __instance;
            if (apparel.TryGetComp<CompApparelWithAltStateGraphics>() is CompApparelWithAltStateGraphics comp && comp.isAltState) 
            {
                __result = comp.Props.altStateWornGraphicPath;
            }
        }
    }
}
