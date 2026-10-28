using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection.Emit;
using Verse;

namespace MousekinRace
{
    // Conditionally patch leader and moral guide titles for Mousekin player faction
    // for ideo ritual obligation role inactive letter labels and descriptions
    [HarmonyPatch(typeof(Precept_Ritual), nameof(Precept_Ritual.AddObligation))]
    public static class Harmony_Precept_Ritual_AddObligation_ReplaceRoleTitlesForMousekinPlayer
    {
        private static Precept_Role CachedRole;

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> GetRole_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codeMatcher = new CodeMatcher(instructions);

            CodeMatch[] toMatch =
            [
                new CodeMatch(OpCodes.Ldloc_3),
                new CodeMatch(OpCodes.Brfalse_S)
            ];

            CodeInstruction[] toInsert =
            [
                new CodeInstruction(OpCodes.Ldloc_2),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Harmony_Precept_Ritual_AddObligation_ReplaceRoleTitlesForMousekinPlayer), nameof(Harmony_Precept_Ritual_AddObligation_ReplaceRoleTitlesForMousekinPlayer.GetRole))),
            ];

            codeMatcher.MatchEndForward(toMatch);
            codeMatcher.Advance(); // make sure we actually move beyond the matcher
            codeMatcher.Insert(toInsert);

            return codeMatcher.InstructionEnumeration();
        }
        
        // Replace the existing ReceiveLetter() method with our own version (see below)
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> ObligationRoleInactiveLetter_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var m_ReceiveLetter = AccessTools.Method(typeof(LetterStack), nameof(LetterStack.ReceiveLetter), [typeof(TaggedString), typeof(TaggedString), typeof(LetterDef), typeof(string), typeof(int), typeof(bool)]);
            var m_ReceiveLetterRoleTitlesReplaced = SymbolExtensions.GetMethodInfo(() => ReceiveLetterRoleTitlesReplaced);

            return instructions.MethodReplacer(m_ReceiveLetter, m_ReceiveLetterRoleTitlesReplaced);
        }

        private static void GetRole(Precept_Role role)
        {
            CachedRole = role;
        }

        private static void ReceiveLetterRoleTitlesReplaced(LetterStack stack, TaggedString label, TaggedString text, LetterDef textLetterDef, string debugInfo = null, int delayTicks = 0, bool playSound = true)
        {
            // Perform substitutions in both the letter label and description
            label = CachedRole != null ? Utils.ReplaceIdeoRoleTitlesForMousekinPlayer(label, CachedRole) : label;
            text = CachedRole != null ? Utils.ReplaceIdeoRoleTitlesForMousekinPlayer(text, CachedRole) : text;

            // Feed the updated text back into the standard ReceiveLetter() method
            stack.ReceiveLetter(label, text, textLetterDef, debugInfo, delayTicks);
        }
    }

    // Conditionally patch leader and moral guide titles for Mousekin player faction
    // for ritual gizmo roles description list
    [HarmonyPatch(typeof(Precept_Ritual), nameof(Precept_Ritual.RolesDescription))]
    public static class Harmony_Precept_Ritual_RolesDescription_ReplaceRoleTitlesForMousekinPlayer
    {
        static void Postfix(ref string __result, ref Precept_Ritual __instance)
        {
            if (__result != null) 
            {
                foreach (RitualRole role in __instance.behavior.def.roles) 
                {
                    __result = Utils.ReplaceIdeoRoleTitlesForMousekinPlayer(__result, role.FindInstance(__instance.ideo));
                }
            }
        }
    }
}
