using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace MousekinRace
{
    // At world gen faction setup, use the custom ScenPart_RequiredFaction to prevent the removal of a specified faction for the scenario
    [HarmonyPatch(typeof(WorldFactionsUIUtility), nameof(WorldFactionsUIUtility.DoRow))]
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

            codeMatcher.MatchEndForward(toMatch);
            object loopHead = codeMatcher.Instruction.operand; // caching the head of the foreach loop

            CodeInstruction[] toInsert =
            [
                new CodeInstruction(OpCodes.Ldloca_S, 3),
                new CodeInstruction(OpCodes.Call, typeof(List<ScenPart>.Enumerator).Method("get_Current")),
                new CodeInstruction(OpCodes.Ldarg_1), // FactionDef faction
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Harmony_WorldFactionsUIUtility_DoRow_ScenarioRequiredFactionExtraCond), nameof(CheckCurrentScenPartForRequiredFaction))),
                new CodeInstruction(OpCodes.Brfalse_S, loopHead) // go back to top of loop if CheckCurrentScenPartForRequiredFaction also returns false
            ];

            object originalIfBody = codeMatcher.Instruction.operand; // the start of the original if() body
            codeMatcher.Start().MatchEndForward(toMatch);
            codeMatcher.Opcode = OpCodes.Beq_S; // adds logical OR after "if (part.def.preventRemovalOfFaction == factionDef)"
            codeMatcher.Instruction.operand = originalIfBody; // skip executing the second half of new logical OR conditional if first (original) half is true
            codeMatcher.Advance(1); // make sure we actually move beyond the previous conditon check
            codeMatcher.Insert(toInsert);       

            return codeMatcher.InstructionEnumeration();
        }

        private static bool CheckCurrentScenPartForRequiredFaction(ScenPart scenPart, FactionDef faction)
        {
            return scenPart is ScenPart_RequiredFaction scenPart_RequiredFaction && scenPart_RequiredFaction.factionDef == faction;
        }
    }
}