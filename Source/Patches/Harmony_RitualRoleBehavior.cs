using HarmonyLib;
using RimWorld;
using Verse;

namespace MousekinRace
{
    // If the ritual focus target of a Role Change ritual is a Mousekin Town Square, reposition the pawn undertaking
    // the AcceptRole duty to stand at a pre-calculated speaker position cell instead (i.e. in front of the flagpole)
    [HarmonyPatch(typeof(RitualRoleBehavior), nameof(RitualRoleBehavior.GetPosition))]
    public class Harmony_RitualRoleBehavior_GetPosition_AcceptRoleDutyOverrideLocationForTownSquare
    {
        static void Postfix(ref RitualPosition __result, RitualRoleBehavior __instance, Pawn p, LordJob_Ritual ritual)
        {
            if (__instance.dutyDef == MousekinDefOf.AcceptRole && ritual.selectedTarget.Thing is Building_TownSquare && __instance.CustomPositionsForReading.NullOrEmpty())
            {
                __result = new RitualPosition_BesideThing(); // patched elsewhere to correctly handle Town Squares
            }
        }
    }
}
