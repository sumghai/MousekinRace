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
            if (c.GetFirstThingWithComp<CompCellarOutdoor>(__instance.map) is not null)
            { 
                outPlants.RemoveAll(t => t.plant.IsTree);
            }
        }
    }

    // When spawning wild plants, ignore cover checks on cells occupied by a Root Cellar building,
    // by replacing the vanilla GridsUtility.GetCover() method call with a custom method that
    // makes a logical && between GetCover() and a check to see if the cell is occupied by a thing
    // with the custom root cellar comp
    [HarmonyPatch(typeof(WildPlantSpawner), nameof(WildPlantSpawner.CheckSpawnWildPlantAt))]
    public static class Harmony_WildPlantSpawner_CheckSpawnWildPlantAt_RootCellarsIgnoreCoverCheck
    {
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> RootCellarsIgnoreCoverCheck(IEnumerable<CodeInstruction> instructions)
        { 
            var codeMatcher = new CodeMatcher(instructions);

            CodeMatch[] toMatch = new CodeMatch[]
            {
                new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(GridsUtility), nameof(GridsUtility.GetCover)))
            };

            CodeInstruction[] toInsert = new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Harmony_WildPlantSpawner_CheckSpawnWildPlantAt_RootCellarsIgnoreCoverCheck), nameof(Harmony_WildPlantSpawner_CheckSpawnWildPlantAt_RootCellarsIgnoreCoverCheck.HasCoverIgnoreRootCellars)))
            };

            codeMatcher.MatchStartForward(toMatch);
            codeMatcher.RemoveInstruction();
            codeMatcher.Insert(toInsert);

            return codeMatcher.InstructionEnumeration();
        }

        private static bool HasCoverIgnoreRootCellars(IntVec3 c, Map map)
        {
            return c.GetCover(map) != null && c.GetFirstThingWithComp<CompCellarOutdoor>(map) is null;
        }
    }
}
