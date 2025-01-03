using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    [HarmonyPatch(typeof(Designator_Build), nameof(Designator_Build.DrawPanelReadout))]
    public static class Harmony_Designator_Build_DrawPanelReadout_MousekinXmasTreePatch
    {
        static BuildableDef buildableDef;
        
        public static void Prefix(ref Designator_Build __instance)
        {
            buildableDef = __instance.entDef;
        }
        
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> MinifiedTreeTypeFilter_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codeMatcher = new CodeMatcher(instructions);

            CodeMatch[] toMatch =
            [
                new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Widgets), nameof(Widgets.Label), new Type[]{ typeof(Rect), typeof(string) })),
                new CodeMatch(OpCodes.Ldarg_1)
            ];

            CodeInstruction[] toInsert =
            [
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Harmony_Designator_Build_DrawPanelReadout_MousekinXmasTreePatch), nameof(PatchCostList)))
            ];

            codeMatcher.MatchStartForward(toMatch);
            codeMatcher.Insert(toInsert);

            return codeMatcher.InstructionEnumeration();
        }

        private static string PatchCostList(string input)
        {
            return buildableDef == MousekinDefOf.Mousekin_IdeoXmasTree ? input.PatchMinifiedTreeWithPineTreeSuffix() : input;
        }
    }
}
