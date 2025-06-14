using HarmonyLib;
using RimWorld;

namespace MousekinRace
{
    // Skip generating wimp and deserter intro quests from the Shattered Empire if the player has pledged allegiance to a Mousekin faction
    [HarmonyPatch(typeof(IncidentWorker_GiveQuest), nameof(IncidentWorker_GiveQuest.CanFireNowSub))]
    public static class Harmony_IncidentWorker_GiveQuest
    {
        static void Postfix(IncidentWorker_GiveQuest __instance, ref bool __result)
        { 
            /*if (GameComponent_Allegiance.Instance.alignedFaction != null && Faction.OfEmpire != null) 
            {
                if (__instance.def == IncidentDef.Named("GiveQuest_Intro_Wimp") || __instance.def == IncidentDef.Named("GiveQuest_Intro_Deserter"))
                {
                    __result = false;
                }
            }*/
        }
    }
}
