using HarmonyLib;
using RimWorld;
using Verse;

namespace MousekinRace
{
    // Conditionally patch leader and moral guide titles for Mousekin player faction
    // for pawn expectation thoughts
    [HarmonyPatch(typeof(ThoughtWorker_Expectations), nameof(ThoughtWorker_Expectations.PostProcessDescription))]
    public static class Harmony_ThoughtWorker_Expectations_PostProcessDescription_ReplaceRoleTitlesForMousekinPlayer
    {
        static void Postfix(ref string __result, Pawn p)
        {
            if (ModsConfig.IdeologyActive && p.Ideo?.GetRole(p) is Precept_Role role)
            {
                __result = Utils.ReplaceIdeoRoleTitlesForMousekinPlayer(__result, role);
            }
        }
    }
}
