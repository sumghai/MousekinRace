using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace MousekinRace
{
    // Conditionally patch leader and moral guide titles for Mousekin player faction
    // for ideo role lost letter labels and descriptions
    [HarmonyPatch]
    public static class Harmony_Precept_RoleSingle_Assign_ReplaceRoleTitlesForMousekinPlayer
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(Precept_RoleSingle), nameof(Precept_RoleSingle.Assign));
            yield return AccessTools.Method(typeof(Precept_RoleSingle), nameof(Precept_RoleSingle.RecacheActivity));
        }

        private static Precept_Role role;

        // Pre-fetch the role data
        static void Prefix(ref Precept_RoleSingle __instance)
        {
            role = __instance;
        }
        
        // Replace the existing ReceiveLetter() method with our own version (see below)
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> RoleLostLetter_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var m_ReceiveLetter = AccessTools.Method(typeof(LetterStack), nameof(LetterStack.ReceiveLetter), [typeof(TaggedString), typeof(TaggedString), typeof(LetterDef), typeof(LookTargets), typeof(Faction), typeof(Quest), typeof(List<ThingDef>), typeof(string), typeof(int), typeof(bool)]);
            var m_ReceiveLetterRoleTitlesReplaced = SymbolExtensions.GetMethodInfo(() => ReceiveLetterRoleTitlesReplaced);

            return instructions.MethodReplacer(m_ReceiveLetter, m_ReceiveLetterRoleTitlesReplaced);
        }

        private static void ReceiveLetterRoleTitlesReplaced(LetterStack stack, TaggedString label, TaggedString text, LetterDef textLetterDef, LookTargets lookTargets, Faction relatedFaction = null, Quest quest = null, List<ThingDef> hyperlinkThingDefs = null, string debugInfo = null, int delayTicks = 0, bool playSound = true)
        {
            // Perform substitutions in both the letter label and description
            label = role != null ? Utils.ReplaceIdeoRoleTitlesForMousekinPlayer(label, role, true) : label;
            text = role != null ? Utils.ReplaceIdeoRoleTitlesForMousekinPlayer(text, role, true) : text;

            // Feed the updated text back into the standard ReceiveLetter() method
            stack.ReceiveLetter(label, text, textLetterDef, lookTargets, relatedFaction, quest, hyperlinkThingDefs, debugInfo, delayTicks);
        }
    }

    // Conditionally patch leader and moral guide titles for Mousekin player faction
    // for ideo role active/inactive and ritual skipped letter labels and descriptions
    [HarmonyPatch]
    public static class Harmony_Precept_RoleSingle_RecacheActivity_ReplaceRoleTitlesForMousekinPlayer
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(Precept_Ritual), nameof(Precept_Ritual.AddObligation));
            yield return AccessTools.Method(typeof(Precept_RoleSingle), nameof(Precept_RoleSingle.RecacheActivity));
        }

        private static Precept_Role role;

        // Pre-fetch the role data
        static void Prefix(ref Precept_RoleSingle __instance)
        {
            role = __instance;
        }

        // Replace the existing ReceiveLetter() method with our own version (see below)
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> RoleActiveInactiveLetter_Transpiler(IEnumerable<CodeInstruction> instructions)
        {           
            var m_ReceiveLetter = AccessTools.Method(typeof(LetterStack), nameof(LetterStack.ReceiveLetter), [typeof(TaggedString), typeof(TaggedString), typeof(LetterDef), typeof(string), typeof(int), typeof(bool)]);
            var m_ReceiveLetterRoleTitlesReplaced = SymbolExtensions.GetMethodInfo(() => ReceiveLetterRoleTitlesReplaced);

            return instructions.MethodReplacer(m_ReceiveLetter, m_ReceiveLetterRoleTitlesReplaced);
        }

        private static void ReceiveLetterRoleTitlesReplaced(LetterStack stack, TaggedString label, TaggedString text, LetterDef textLetterDef, string debugInfo = null, int delayTicks = 0, bool playSound = true)
        {
            // Perform substitutions in both the letter label and description
            label = role != null ? Utils.ReplaceIdeoRoleTitlesForMousekinPlayer(label, role) : label;
            text = role != null ? Utils.ReplaceIdeoRoleTitlesForMousekinPlayer(text, role) : text;

            // Feed the updated text back into the standard ReceiveLetter() method
            stack.ReceiveLetter(label, text, textLetterDef, debugInfo, delayTicks);
        }
    }
}
