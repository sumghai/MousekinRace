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
            yield return AccessTools.PropertyGetter(typeof(Thought_IdeoRoleApparelRequirementNotMet), nameof(Thought_IdeoRoleApparelRequirementNotMet.LabelCap));
            yield return AccessTools.PropertyGetter(typeof(Thought_IdeoRoleApparelRequirementNotMet), nameof(Thought_IdeoRoleApparelRequirementNotMet.Description));
            yield return AccessTools.PropertyGetter(typeof(Thought_IdeoRoleEmpty), nameof(Thought_IdeoRoleEmpty.LabelCap));
            yield return AccessTools.PropertyGetter(typeof(Thought_IdeoRoleEmpty), nameof(Thought_IdeoRoleEmpty.Description));
            yield return AccessTools.PropertyGetter(typeof(Thought_IdeoRoleLost), nameof(Thought_IdeoRoleLost.LabelCap));
            yield return AccessTools.PropertyGetter(typeof(Thought_IdeoRoleLost), nameof(Thought_IdeoRoleLost.Description));
        }

        public static void Postfix(ref string __result, object __instance)
        {
            // Use runtime type matching to retrieve the role from the appropriate thought instance
            Precept_Role role = __instance switch
            {
                Thought_IdeoRoleApparelRequirementNotMet t => t.Role,
                Thought_IdeoRoleEmpty t => t.Role,
                Thought_IdeoRoleLost t => t.Role,
                _ => null
            };

            __result = Utils.ReplaceIdeoRoleTitlesForMousekinPlayer(__result, role);
        }
    }
}
