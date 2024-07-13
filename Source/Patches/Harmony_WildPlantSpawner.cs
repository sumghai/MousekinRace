using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection.Emit;
using Verse;

namespace MousekinRace
{
    // When spawning wild plants, never spawn trees on cells occupied by a Root Cellar building
    [HarmonyPatch(typeof(WildPlantSpawner), nameof(WildPlantSpawner.CalculatePlantsWhichCanGrowAt))]
    public static class Harmony_WildPlantSpawner_CalculatePlantsWhichCanGrowAt_RootCellarsNoTrees
    {
        static void Postfix(WildPlantSpawner __instance, IntVec3 c, ref List<ThingDef> outPlants)
        {
            if (c.GetFirstThing<Building_CellarOutdoor>(__instance.map) is not null)
            { 
                outPlants.RemoveAll(t => t.plant.IsTree);
            }
        }
    }

    // When spawning wild plants, ignore cover checks on cells occupied by a Root Cellar building,
    // by replacing the vanilla GridsUtility.GetCover() and GridsUtility.GetEdifice() method calls
    // with custom methods that makes a logical && between GetCover()/GetEdifice() and a check
    // to see if the cell is occupied by a Root Cellar building
    [HarmonyPatch(typeof(WildPlantSpawner), nameof(WildPlantSpawner.CheckSpawnWildPlantAt))]
    public static class Harmony_WildPlantSpawner_CheckSpawnWildPlantAt_RootCellarsIgnoreCoverAndEdificeCheck
    {
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> RootCellarsIgnoreCoverAndEdificeCheck(IEnumerable<CodeInstruction> instructions)
        { 
            var codeMatcher = new CodeMatcher(instructions);

            CodeMatch[] toMatch_GetCover = new CodeMatch[]
            {
                new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(GridsUtility), nameof(GridsUtility.GetCover)))
            };

            CodeInstruction[] toInsert_GetCover = new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Harmony_WildPlantSpawner_CheckSpawnWildPlantAt_RootCellarsIgnoreCoverAndEdificeCheck), nameof(GetCoverIgnoreRootCellars)))
            };

            codeMatcher.MatchStartForward(toMatch_GetCover);
            codeMatcher.RemoveInstruction();
            codeMatcher.Insert(toInsert_GetCover);

            CodeMatch[] toMatch_GetEdifice = new CodeMatch[]
            {
                new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(GridsUtility), nameof(GridsUtility.GetEdifice)))
            };

            CodeInstruction[] toInsert_GetEdifice = new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Harmony_WildPlantSpawner_CheckSpawnWildPlantAt_RootCellarsIgnoreCoverAndEdificeCheck), nameof(GetEdificeIgnoreRootCellars)))
            };

            codeMatcher.MatchStartForward(toMatch_GetEdifice);
            codeMatcher.RemoveInstruction();
            codeMatcher.Insert(toInsert_GetEdifice);

            return codeMatcher.InstructionEnumeration();
        }

        private static bool GetCoverIgnoreRootCellars(IntVec3 c, Map map)
        {
            return c.GetCover(map) != null && c.GetCover(map) is not Building_CellarOutdoor;
        }

        private static bool GetEdificeIgnoreRootCellars(IntVec3 c, Map map)
        {
            return c.GetEdifice(map) != null && c.GetEdifice(map) is not Building_CellarOutdoor;
        }
    }
}
