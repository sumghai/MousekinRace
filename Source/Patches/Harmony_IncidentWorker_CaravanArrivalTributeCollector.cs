using HarmonyLib;
using RimWorld;

namespace MousekinRace
{
    // Disable Shattered Empire tribute collector if the player has pledged allegiance to a Mousekin faction
    [HarmonyPatch(typeof(IncidentWorker_CaravanArrivalTributeCollector), nameof(IncidentWorker_CaravanArrivalTributeCollector.CanFireNowSub))]
    public static class Harmony_IncidentWorker_CaravanArrivalTributeCollector_CanFireNowSub_DisableIfAllegianceSet
    {
        /*static void Postfix(ref bool __result)
        {
            if (GameComponent_Allegiance.Instance.alignedFaction != null) 
            {
                __result = false;
            }
        }*/
    }
}
