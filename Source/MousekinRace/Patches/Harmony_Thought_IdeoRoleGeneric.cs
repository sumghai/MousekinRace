using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;

namespace MousekinRace
{
    // Conditionally patch leader and moral guide titles for Mousekin player faction
    // for ideo role-related thoughts
    [HarmonyPatch]
    public static class Harmony_Thought_IdeoRoleGeneric_ReplaceRoleTitlesForMousekinPlayer
    {
        public static IEnumerable<MethodBase> TargetMethods()
        {
            // TODO - Fix for RW 1.7 (commented out lines)

            //yield return AccessTools.PropertyGetter(typeof(Thought_IdeoRoleApparelRequirementNotMet), nameof(Thought_IdeoRoleApparelRequirementNotMet.LabelCap));
            //yield return AccessTools.PropertyGetter(typeof(Thought_IdeoRoleApparelRequirementNotMet), nameof(Thought_IdeoRoleApparelRequirementNotMet.Description));
            yield return AccessTools.PropertyGetter(typeof(Thought_IdeoRoleEmpty), nameof(Thought_IdeoRoleEmpty.LabelCap));
            yield return AccessTools.PropertyGetter(typeof(Thought_IdeoRoleEmpty), nameof(Thought_IdeoRoleEmpty.Description));
            //yield return AccessTools.PropertyGetter(typeof(Thought_IdeoRoleLost), nameof(Thought_IdeoRoleLost.LabelCap));
            //yield return AccessTools.PropertyGetter(typeof(Thought_IdeoRoleLost), nameof(Thought_IdeoRoleLost.Description));
        }

        public static void Postfix(ref string __result, Thought_IdeoRoleEmpty __instance)
        {
            __result = Utils.ReplaceIdeoRoleTitlesForMousekinPlayer(__result, __instance.Role);
        }
    }
}
