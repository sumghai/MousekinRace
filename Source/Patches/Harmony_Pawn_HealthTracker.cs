using HarmonyLib;
using Verse;

namespace MousekinRace.Patches
{
    [HarmonyPatch(typeof(Pawn_HealthTracker), nameof(Pawn_HealthTracker.NotifyPlayerOfKilled))]
    public static class Harmony_Pawn_HealthTracker_NotifyPlayerOfKilled_OverrideLetterMessage
    {
        static void Prefix(ref DamageInfo? dinfo, Hediff hediff)
        {
            // Forces Mousekins killed by hemlock poisoning to always display the letter from the corresponding DamageDef
            if (hediff.def == MousekinDefOf.Mousekin_HemlockPoisoning && !dinfo.HasValue)
            {
                dinfo = new DamageInfo(MousekinDefOf.Mousekin_SuicidePoison, 1f, instigatorGuilty: false);
            }
        }
    }
}
