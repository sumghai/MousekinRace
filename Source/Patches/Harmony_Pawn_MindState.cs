using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace MousekinRace
{
    // Put earless Mousekins in a suicidal state once they have reached the critical time threshold
    [HarmonyPatch(typeof(Pawn_MindState), nameof(Pawn_MindState.MindStateTick))]
    public static class Harmony_Pawn_MindState_EarlessMousekinSuicideWatcher
    {
        static void Postfix(Pawn_MindState __instance)
        {
            // Skip if earless suicide mechanic is disabled
            if (!MousekinRaceMod.Settings.EarlessMousekinsAreSuicidal)
            {
                return;
            }

            Pawn pawn = __instance.pawn;

            // Skip for pawns not on map
            if (!pawn.Spawned)
            {
                return;
            }

            // Only check every six hours
            if (pawn.IsHashIntervalTick(15000))
            {
                // Only check Mousekin pawns
                if (Utils.IsMousekin(pawn))
                {
                    // Only check if the Mousekin has lost both ears
                    if (EarlessMousekinAlertUtility.IsMissingBothEars(pawn))
                    {
                        // Only check if the time has elapsed past the suicide threshold
                        if (EarlessMousekinAlertUtility.GetDaysSinceBothEarsLost(pawn) > EarlessMousekinAlertUtility.suicideAttemptThresholdDays)
                        {
                            // Only trigger if the Mousekin belongs to the player faction and is not already a(nother) mental state
                            if (!pawn.InMentalState && pawn.Faction == Faction.OfPlayer && MousekinDefOf.Mousekin_MentalState_EarlessSuicide.Worker.StateCanOccur(pawn))
                            {
                                pawn.mindState.mentalStateHandler.TryStartMentalState(MousekinDefOf.Mousekin_MentalState_EarlessSuicide);
                            }
                        }
                    }
                }
            }
        }
    }
}
