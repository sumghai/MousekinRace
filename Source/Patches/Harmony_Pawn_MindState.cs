using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace MousekinRace.Patches
{
    // Put earless Mousekins in a suicidal state once they have reached the critical time threshold
    [HarmonyPatch(typeof(Pawn_MindState), nameof(Pawn_MindState.MindStateTick))]
    public static class Harmony_Pawn_MindState_EarlessMousekinSuicideWatcher
    {
        static void Postfix(Pawn_MindState __instance)
        {
            Pawn pawn = __instance.pawn;
            if (Utils.IsMousekin(pawn) && !pawn.InMentalState && pawn.Faction == Faction.OfPlayer && EarlessMousekinAlertUtility.GetDaysSinceBothEarsLost(pawn) > EarlessMousekinAlertUtility.suicideAttemptThresholdDays && MousekinDefOf.Mousekin_MentalState_EarlessSuicide.Worker.StateCanOccur(pawn) && MousekinRaceMod.Settings.EarlessMousekinsAreSuicidal)
            {
                pawn.mindState.mentalStateHandler.TryStartMentalState(MousekinDefOf.Mousekin_MentalState_EarlessSuicide);
            }
        }
    }
}
