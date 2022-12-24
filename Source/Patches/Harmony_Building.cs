using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MousekinRace
{
    // Destroy wall-mounted things when the wall itself is destroyed
    [HarmonyPatch(typeof(Building), nameof(Building.Destroy))]
    public static class Harmony_Building_Destroy_DestroyWallMountedThings
    {
        public static void Prefix(Building __instance, DestroyMode mode)
        {
            if (__instance != null && __instance.def != null && __instance.def.passability == Traversability.Impassable && __instance.Map != null)
            {
                // Search adjacent cells in all four cardinal directions
                List<IntVec3> adjacentCells = GetAdjacentCells(__instance.Position);

                foreach (IntVec3 cell in adjacentCells)
                {
                    // Only destroy wall-mounted things whose facing cells match the wall being destroyed
                    (from m in cell.GetThingList(__instance.Map)
                     where m != __instance && m.def.placeWorkers != null && m.def.placeWorkers.Contains(typeof(PlaceWorker_WallMounted)) && (m.Position + m.Rotation.FacingCell) == __instance.Position
                     select m).ToList().ForEach(delegate (Thing x)
                    {
                        x.Destroy(mode);
                    });
                }
            }
        }

        public static List<IntVec3> GetAdjacentCells(IntVec3 cell)
        {
            IntVec3 north = cell + IntVec3.North;
            IntVec3 east = cell + IntVec3.East;
            IntVec3 south = cell + IntVec3.South;
            IntVec3 west = cell + IntVec3.West;
            return new List<IntVec3>() { north, east, south, west };
        }
    }

    // Destroy wall-mounted things when an impassable wall is placed directly onto its cell
    [HarmonyPatch(typeof(Building), nameof(Building.SpawnSetup))]
    public static class Harmony_Building_SpawnSetup_DestroyWallMountedThingsWithImpassable
    {
        public static void Postfix(Building __instance)
        {
            if (__instance.IsWall())
            {
                (from m in __instance.Position.GetThingList(__instance.Map)
                 where m != __instance & m.def.placeWorkers != null && m.def.placeWorkers.Contains(typeof(PlaceWorker_WallMounted))
                 select m).ToList().ForEach(delegate (Thing x)
                 {
                     x.Destroy();
                 });
            }
        }

        public static bool IsWall(this Thing thing)
        {
            return thing.Position.GetThingList(thing.Map)
                .Where(t => t.def.category == ThingCategory.Building)
                .Where(t => t.def.building != null)
                .Where(t => !t.def.building.isNaturalRock)
                .Any(t => t.def.passability == Traversability.Impassable);
        }
    }
}
