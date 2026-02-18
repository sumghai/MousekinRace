using HarmonyLib;
using RimWorld;
using System.Linq;
using Verse;

namespace MousekinRace
{
    // Conditionally patch moral guide title for Mousekin player faction
    // for conversion ritual float menu option messages
    [HarmonyPatch(typeof(RitualBehaviorWorker_Conversion), nameof(RitualBehaviorWorker_Conversion.CanStartRitualNow))]
    public static class Harmony_RitualBehaviorWorker_Conversion_ReplaceRoleTitlesForMousekinPlayer
    {
        static void Postfix(ref string __result, Precept_Ritual ritual)
        {
            Precept_Role role = ritual.ideo.RolesListForReading.FirstOrDefault((Precept_Role r) => r.def == PreceptDefOf.IdeoRole_Moralist);

            if (__result != null && role != null && role.ideo.culture.IsMousekinKingdomLike())
            {
                string tempOutput = __result;
                __result = tempOutput.Replace(role?.LabelCap, MousekinDefOf.MousekinPriest.label.Replace(MousekinDefOf.Mousekin.label, "").Trim().CapitalizeFirst());
            }
        }
    }
}