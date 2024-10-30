using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace MousekinRace
{
    // Conditionally patch leader and moral guide titles for Mousekin player faction
    // for pawn portrait ideo role icon in ritual dialog
    [HarmonyPatch(typeof(PawnPortraitIconsDrawer), nameof(PawnPortraitIconsDrawer.CalculatePawnPortraitIcons))]
    public static class Harmony_PawnPortraitIconsDrawer_CalculatePawnPortraitIcons_ReplaceRoleTitlesForMousekinPlayer
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> IdeoRoleIconTooltip_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codeMatcher = new CodeMatcher(instructions);

            CodeMatch[] toMatch =
            [
                new CodeMatch(OpCodes.Ldloc_S),
                new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Precept), nameof(Precept.TipLabel)))
            ];

            CodeInstruction[] toInsert =
            [
                new CodeInstruction(OpCodes.Ldloc_S, 4), // Adds another instance of Precept_Role role to the stack
                new CodeInstruction(OpCodes.Ldc_I4_0), // false (do not replace indefinite article)
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Utils), nameof(Utils.ReplaceIdeoRoleTitlesForMousekinPlayer))) // Feeds the original unaltered Precept.TipLabel (as above) and the extra instance of role from the stack into our helper method for replacing Mousekin player ideo role titles
            ];

            codeMatcher.MatchEndForward(toMatch).Advance(1); // Make sure we actually move beyond Precept.TipLabel in the stack
            codeMatcher.Insert(toInsert);

            return codeMatcher.InstructionEnumeration();
        }
    }
}
