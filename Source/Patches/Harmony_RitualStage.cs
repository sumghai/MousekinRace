using HarmonyLib;
using RimWorld;
using Verse.AI;

namespace MousekinRace
{
    // If a ritual focus target is a Mousekin Town Square, and the default duty for any given ritual stage is SpectateCircle,
    // dynamically change to the regular Spectate instead (which has been patched elsewhere to correctly handle Town Squares)
    [HarmonyPatch(typeof(RitualStage), nameof(RitualStage.GetDuty))]
    public class Harmony_RitualStage_GetDuty_OverrideSpectateDutyForTownSquare
    {
        static void Postfix(ref DutyDef __result, LordJob_Ritual ritual)
        {
            if (ritual.selectedTarget.Thing is Building_TownSquare && __result == MousekinDefOf.SpectateCircle)
            {
                __result = DutyDefOf.Spectate;
            }
        }
    }
}
