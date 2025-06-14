using HarmonyLib;
using RimWorld;
using RimWorld.QuestGen;

namespace MousekinRace
{
    // Skip generating quests from the Shattered Empire if the player has pledged allegiance to a Mousekin faction
    [HarmonyPatch(typeof(QuestNode_GetFaction), nameof(QuestNode_GetFaction.IsGoodFaction))]
    public static class Harmony_QuestNode_GetFaction_IsGoodFaction_SkipEmpireIfAllegianceSet
    {
        public static void Postfix(ref bool __result, Faction faction)
        {
            /*if (GameComponent_Allegiance.Instance.alignedFaction != null && faction == Faction.OfEmpire)
            {
                __result = false;
            }*/
        }
    }
}
