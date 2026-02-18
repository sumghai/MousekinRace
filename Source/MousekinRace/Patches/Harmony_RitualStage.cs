using HarmonyLib;
using RimWorld;
using Verse.AI;

namespace MousekinRace
{
    // If a ritual focus target is a Mousekin Town Square, conditionally change duties to different ones
    // that have been patched elsewhere to correctly handle Town Squares:
    // - SpectateCircle -> Spectate
    // - DeliverPawnToAltar -> DeliverPawnToCell
    [HarmonyPatch(typeof(RitualStage), nameof(RitualStage.GetDuty))]
    public class Harmony_RitualStage_GetDuty_OverrideDutiesForTownSquare
    {
        static void Postfix(ref DutyDef __result, LordJob_Ritual ritual)
        {
            if (ritual.selectedTarget.Thing is Building_TownSquare)
            {
                if (__result == MousekinDefOf.SpectateCircle)
                {
                    __result = DutyDefOf.Spectate;
                }

                if (__result == MousekinDefOf.DeliverPawnToAltar)
                {
                    __result = MousekinDefOf.DeliverPawnToCell;
                }
            }
        }
    }
}
