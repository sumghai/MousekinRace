using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Verse;

namespace MousekinRace
{
    // At world gen faction setup, use the custom ScenPart_RequiredFaction to prevent the removal of a specified faction for the scenario
    /*[HarmonyPatch(typeof(WorldFactionsUIUtility), nameof(WorldFactionsUIUtility.DoRow))]
    public static class Harmony_WorldFactionsUIUtility_DoRow_ScenarioRequiredFactionExtraCond
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> ScenarioRequiredFactionExtraCond_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codeMatcher = new CodeMatcher(instructions);

            CodeMatch[] toMatch =
            [
                new CodeMatch(OpCodes.Ldfld),
                new CodeMatch(OpCodes.Ldfld),
                new CodeMatch(OpCodes.Ldarg_1),
                new CodeMatch(OpCodes.Bne_Un_S)
            ];

            CodeInstruction[] toInsert =
            [
                new CodeInstruction(OpCodes.Call, typeof(ScenPart).GetMethod("get_Current()")),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Harmony_WorldFactionsUIUtility_DoRow_ScenarioRequiredFactionExtraCond), nameof(Harmony_WorldFactionsUIUtility_DoRow_ScenarioRequiredFactionExtraCond.CheckCurrentScenPartForRequiredFaction))),
                new CodeInstruction(OpCodes.Brfalse)
            ];

            codeMatcher.MatchEndForward(toMatch);
            if (!codeMatcher.IsInvalid)
            { 
                toInsert.Last().operand = codeMatcher.Instruction.operand;
            }
            codeMatcher.Insert(toInsert);

            return codeMatcher.InstructionEnumeration();
        }

        private static bool CheckCurrentScenPartForRequiredFaction(ScenPart scenPart, FactionDef faction)
        {
            Log.Warning($"Running check for {faction}");
            return scenPart is ScenPart_RequiredFaction scenPart_RequiredFaction && scenPart_RequiredFaction.factionDef == faction;
        }
    }*/
}