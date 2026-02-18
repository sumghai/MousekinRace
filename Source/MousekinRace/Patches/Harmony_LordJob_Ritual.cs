using HarmonyLib;
using RimWorld;

namespace MousekinRace
{
    // If the ritual focus target of a Prisoner Execution ritual is a Mousekin Town Square, change the end trigger 
    // for the first stage of the ritual from StageEndTrigger_PawnDeliveredOrNotValid to StageEndTrigger_RolesArrived
    // - StageEndTrigger_PawnDeliveredOrNotValid requires an altar building
    // - StageEndTrigger_RolesArrived allows a target cell instead (patched elsewhere)
    [HarmonyPatch(typeof(LordJob_Ritual), nameof(LordJob_Ritual.CreateGraph))]
    public class Harmony_LordJob_Ritual_CreateGraph_StageEndTriggerPrisonerExecutionOverrideForTownSquare
    {
        static void Prefix(ref LordJob_Ritual __instance)
        {
            if (__instance.selectedTarget.Thing is Building_TownSquare) 
            {
                foreach (RitualStage stage in __instance.stages) 
                {
                    for (int i = 0; i < stage.endTriggers.Count; i++)
                    {
                        if (stage.endTriggers[i] is StageEndTrigger_PawnDeliveredOrNotValid)
                        {
                            stage.endTriggers[i] = new StageEndTrigger_RolesArrived()
                            {
                                roleIds = ["prisoner"]
                            };
                        }
                    }
                }
            }
        }
    }
}
