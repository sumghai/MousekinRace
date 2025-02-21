using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace MousekinRace
{
    // If the ritual focus target of a Prisoner Execution ritual is a Mousekin Town Square,
    // change the lie down location of the prisoner to a prec-calculated speaker position cell
    // (the execution will stand just behind them)
    [HarmonyPatch(typeof(JobGiver_LayDownAwake), nameof(JobGiver_LayDownAwake.TryGiveJob))]
    public class Harmony_JobGiver_LayDownAwake_TryGiveJob_PrisonerExecutionOverrideForTownSquare
    {
        static void Prefix(ref Pawn pawn)
        {
            if (pawn.GetLord()?.LordJob is LordJob_Ritual ritual && ritual.PawnWithRole("prisoner") == pawn && pawn.mindState.duty.focusThird.Thing is Building_TownSquare townSquare)
            {
                pawn.mindState.duty.focusThird = townSquare.speechSpeakerCellPos;
            }
        }
    }
}
