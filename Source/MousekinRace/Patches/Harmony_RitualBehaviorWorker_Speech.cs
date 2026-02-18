using HarmonyLib;
using RimWorld;
using System.Linq;
using Verse;

namespace MousekinRace
{
    // Conditionally patch leader title for Mousekin player faction
    // for speech ritual float menu option messages
    [HarmonyPatch(typeof(RitualBehaviorWorker_Speech), nameof(RitualBehaviorWorker_Speech.CanStartRitualNow))]
    public static class Harmony_RitualBehaviorWorker_Speech_ReplaceRoleTitlesForMousekinPlayer
    {
        static void Postfix(ref string __result, Precept_Ritual ritual)
        {
            Precept_Role role = ritual.ideo.RolesListForReading.FirstOrDefault((Precept_Role r) => r.def == PreceptDefOf.IdeoRole_Leader);

            if (__result != null && role != null && role.ideo.culture.IsMousekin())
            {
                string tempOutput = __result;
                __result = tempOutput.Replace(role?.LabelCap, "MousekinRace_PreceptRole_PlayerLeaderTitle".Translate());
            }
        }
    }
}