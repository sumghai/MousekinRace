using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace MousekinRace
{
    // Skip regenerating ritual seats for Mousekin ideos when loading/saving ideo data
    [HarmonyPatch(typeof(Ideo), nameof(Ideo.ExposeData))]
    public static class Harmony_Ideo_ExposeData_PreventRitualSeatRespawnForMousekinIdeos
    {
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> RitualSeatRespawn_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codeMatcher = new CodeMatcher(instructions);

            CodeMatch[] toMatch = new CodeMatch[]
            {
                new CodeMatch(OpCodes.Ldloca_S),
                new CodeMatch(OpCodes.Call),
                new CodeMatch(OpCodes.Brfalse_S)
            };

            codeMatcher.MatchEndForward(toMatch);
            object label = codeMatcher.Instruction.operand;

            CodeInstruction[] toInsert = new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Harmony_Ideo_ExposeData_PreventRitualSeatRespawnForMousekinIdeos), nameof(IdeoIsNotMousekin))),
                new CodeInstruction(OpCodes.Brfalse_S, label)
            };

            codeMatcher.Insert(toInsert);

            return codeMatcher.InstructionEnumeration();
        }

        private static bool IdeoIsNotMousekin(Ideo ideo)
        {
            return !ideo.culture.IsMousekin();
        }
    }
}
