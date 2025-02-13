using HarmonyLib;
using Verse;

namespace MousekinRace
{
    // Prevent heretics being executed in the Purging Flames ritual from crawling to safety
    [HarmonyPatch(typeof(Pawn_HealthTracker), nameof(Pawn_HealthTracker.CanCrawl), MethodType.Getter)]
    public static class Harmony_Pawn_HealthTracker_CanCrawl_SkipForHereticsBeingExecuted
    {
        public static void Postfix(Pawn_HealthTracker __instance, ref bool __result)
        {
            if (__instance.pawn.IsHereticBeingPurgedByFire())
            {
                __result = false;
            }
        }
    }

    // Force Mousekins killed by hemlock poisoning to always display the letter from the corresponding DamageDef
    [HarmonyPatch(typeof(Pawn_HealthTracker), nameof(Pawn_HealthTracker.NotifyPlayerOfKilled))]
    public static class Harmony_Pawn_HealthTracker_NotifyPlayerOfKilled_OverrideLetterMessage
    {
        static void Prefix(ref DamageInfo? dinfo, Hediff hediff)
        {
            if (hediff?.def == MousekinDefOf.Mousekin_HemlockPoisoning && !dinfo.HasValue)
            {
                dinfo = new DamageInfo(MousekinDefOf.Mousekin_SuicidePoison, 1f, instigatorGuilty: false);
            }
        }
    }
}
