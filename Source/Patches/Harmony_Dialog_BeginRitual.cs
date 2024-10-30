using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace MousekinRace
{
    // Conditionally patch leader and moral guide titles for Mousekin player faction
    // for ideo role assign confirmation text in the begin ritual dialog(s)
    [HarmonyPatch]
    public static class Harmony_Dialog_BeginRitual_DrawRoleSelection_AssignConfirmation_ReplaceRoleTitlesForMousekinPlayer
    {
        // Original:
        // confirmText = confirmTextLocal + "\n\n" + extraConfirmText + "\n\n" + "ChooseRoleConfirmAssignPostfix".Translate();
        static MethodInfo TargetMethod() => AccessTools.Method(AccessTools.Inner(typeof(Dialog_BeginRitual), "<>c__DisplayClass52_2"), "<DrawRoleSelection>b__3");

        // Get the internal fields for later use
        static readonly FieldInfo localsField = AccessTools.Field(typeof(Dialog_BeginRitual).GetNestedTypes(AccessTools.all).FirstOrDefault((Type c) => c.Name.Contains("<>c__DisplayClass52_2")), "CS$<>8__locals2");
        static readonly FieldInfo newRoleField = AccessTools.Field(AccessTools.Inner(typeof(Dialog_BeginRitual), "<>c__DisplayClass52_1"), "newRole");

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> AssignConfirmText_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codeMatcher = new CodeMatcher(instructions);

            CodeMatch[] toMatch =
            [
                new CodeMatch(OpCodes.Stfld)
            ];

            CodeInstruction[] toInsert =
            [
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, localsField),
                new CodeInstruction(OpCodes.Ldfld, newRoleField), // Get new role that is about to be assigned to pawn
                new CodeInstruction(OpCodes.Ldc_I4_0), // false (don't remove any indefinite articles)
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Utils), nameof(Utils.ReplaceIdeoRoleTitlesForMousekinPlayer)))    
            ];

            codeMatcher.MatchStartForward(toMatch);
            codeMatcher.Insert(toInsert);

            return codeMatcher.InstructionEnumeration();
        }
    }

    // Conditionally patch leader and moral guide titles for Mousekin player faction
    // for ideo role unassign confirmation text in the begin ritual dialog(s)
    [HarmonyPatch]
    public static class Harmony_Dialog_BeginRitual_DrawRoleSelection_UnassignConfirmation_ReplaceRoleTitlesForMousekinPlayer
    {
        // Original:
        // confirmText = "ChooseRoleConfirmUnassign".Translate(currentRole.Named("ROLE"), pawn.Named("PAWN")) + "\n\n" + "ChooseRoleConfirmAssignPostfix".Translate();
        static MethodInfo TargetMethod() => AccessTools.Method(AccessTools.Inner(typeof(Dialog_BeginRitual), "<>c__DisplayClass52_0"), "<DrawRoleSelection>b__0");

        // Get the internal currentRole field for later use
        static readonly FieldInfo currentRoleField = AccessTools.Field(AccessTools.Inner(typeof(Dialog_BeginRitual), "<>c__DisplayClass52_0"), "currentRole");

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> UnassignConfirmText_Transpiler(IEnumerable<CodeInstruction> instructions)
        {            
            var codeMatcher = new CodeMatcher(instructions);

            CodeMatch[] toMatch =
            [
                new CodeMatch(OpCodes.Stfld)
            ];
            
            CodeInstruction[] toInsert =
            [
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, currentRoleField), // Get current role
                new CodeInstruction(OpCodes.Ldc_I4_1), // true (remove indefinite article)
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Utils), nameof(Utils.ReplaceIdeoRoleTitlesForMousekinPlayer)))
            ];

            codeMatcher.MatchStartForward(toMatch);
            codeMatcher.Insert(toInsert);

            return codeMatcher.InstructionEnumeration();
        }
    }
}
