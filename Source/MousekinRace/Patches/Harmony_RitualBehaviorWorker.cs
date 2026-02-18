using HarmonyLib;
using RimWorld;

namespace MousekinRace
{
    // Conditionally patch moral guide title for Mousekin player faction
    // for miscellaneous ritual float menu option messages
    [HarmonyPatch(typeof(RitualBehaviorWorker), nameof(RitualBehaviorWorker.CanStartRitualNow))]
    public static class Harmony_RitualBehaviorWorker_CanStartRitualNow_ReplaceRoleTitlesForMousekinPlayer
    {
        static void Postfix(ref string __result, Precept_Ritual ritual)
        {
            if (__result != null && ritual.behavior?.def.roles != null)
            {
                foreach (RitualRole role in ritual.behavior.def.roles)
                {
                    __result = Utils.ReplaceIdeoRoleTitlesForMousekinPlayer(__result, role.FindInstance(ritual.ideo));
                }
            }
        }
    }
}
