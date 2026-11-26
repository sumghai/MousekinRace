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

    // Restrict crafting of faction-unique buildings to players whose allegiance matches the building's associated faction
    [HarmonyPatch(typeof(Designator_Build), nameof(Designator_Build.Visible), MethodType.Getter)]
    public static class Harmony_Designator_Build_Visible_RestrictBuildingsByAllegiance
    {
        public static void Postfix(Designator_Build __instance, ref bool __result)
        {
            if (!__result) return; // Skip if building is already hidden from menu

            ThingDef currentDef = __instance.PlacingDef as ThingDef;
            if (MousekinDefOf.FactionRestrictedBuildableThingDefs.Contains(currentDef)) 
            {
                if (GameComponent_Allegiance.Instance.alignedFaction == null || !GameComponent_Allegiance.Instance.alignedFaction.def.GetModExtension<AlliableFactionExtension>().factionRestrictedBuildableThingDefs.Contains(currentDef))
                {
                    __result = false;
                }
            }
        }
    }
}
