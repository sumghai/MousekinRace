using HarmonyLib;
using RimWorld;
using RimWorld.QuestGen;
using Verse;

namespace MousekinRace
{
    // Skip generating quests with pawns from the Shattered Empire if the player has pledged allegiance to a Mousekin faction
    [HarmonyPatch(typeof(QuestNode_GetPawn), nameof(QuestNode_GetPawn.IsGoodPawn))]
    public static class Harmony_QuestNode_GetPawn_IsGoodPawn_SkipEmpireIfAllegianceSet
    {
        public static void Postfix(ref bool __result, Pawn pawn)
        {
            if (GameComponent_Allegiance.Instance.alignedFaction != null && pawn.Faction == Faction.OfEmpire)
            {
                __result = false;
            }
        }
    }
}
