using HarmonyLib;
using RimWorld;
using System.Linq;
using Verse;

namespace MousekinRace
{
    // Recognize when Mousekin Town Squares have been built to fulfill ideo requirements
    [HarmonyPatch(typeof(IdeoBuildingPresenceDemand), nameof(IdeoBuildingPresenceDemand.BuildingPresent))]
    public class Harmony_IdeoBuildingPresenceDemand_BuildingPresent_CheckForTownSquares
    {
        static void Postfix(ref bool __result, Map map, IdeoBuildingPresenceDemand __instance)
        {
            if (__instance.parent.ideo.precepts.Where(p => p is Precept_Building precept_Building && precept_Building.ThingDef == MousekinDefOf.Mousekin_TownSquare).Any() && map.listerBuildings.ColonistsHaveBuilding(MousekinDefOf.Mousekin_TownSquare))
            {
                __result = true;
            }
        }
    }
}
