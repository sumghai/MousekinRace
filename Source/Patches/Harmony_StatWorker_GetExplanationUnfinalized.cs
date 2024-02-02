using HarmonyLib;
using RimWorld;
using Verse;

namespace MousekinRace
{
    // Conditionally hide material/stuff stat multipliers for specially-tagged things
    // (e.g. apparel that is "gilded" rather than 100% made of material/stuff)
    [HarmonyPatch(typeof(StatWorker), nameof(StatWorker.GetExplanationUnfinalized))]
    public class Harmony_StatWorker_GetExplanationUnfinalized
    {
        public static void Postfix(StatWorker __instance, StatRequest req, ref string __result)
        {
            if (req.HasThing && req.StuffDef != null && req.Thing.def.GetModExtension<ApparelIgnoreStuffStatFactorsExtension>() != null && !__instance.stat.parts.NullOrEmpty())
            {
                // Only modify for stats with the custom StatPart
                if (__instance.stat.GetStatPart<StatPart_IgnoreStuffEffectForGildedItems>() != null)
                {
                    float statFactor = req.StuffDef.stuffProps.statFactors.GetStatFactorFromList(__instance.stat);

                    string factorTargetString = "StatsReport_Material".Translate() + " (" + req.StuffDef.LabelCap + "): " + statFactor.ToStringByStyle(ToStringStyle.PercentZero, ToStringNumberSense.Factor);

                    __result = __result.Replace(factorTargetString, string.Empty);
                }
            }
        }
    }
}
