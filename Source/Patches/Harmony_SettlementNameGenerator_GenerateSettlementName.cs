using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MousekinRace
{
    // Rename 1~2 Mousekin Kingdom settlements with custom names
    // - Every kingdom will have one city representing the capital
    // - Large kingdoms will get an extra city representing a major fort
    [HarmonyPatch(typeof(FactionGenerator), nameof(FactionGenerator.GenerateFactionsIntoWorld))]
    public static class Harmony_FactionGenerator_GenerateFactionsIntoWorld_CustomMousekinKingdomSettlementNames
    {
        // Tune this as required
        const int minNumSettlementsForFort = 5;
        
        static void Postfix()
        {
            if (Find.FactionManager.FirstFactionOfDef(MousekinDefOf.Mousekin_FactionKingdom) is Faction kingdomFaction) 
            {
                List<Settlement> kingdomSettlements = Find.WorldObjects.Settlements.Where(x => x.Faction == kingdomFaction).ToList();

                kingdomSettlements.RandomElement().Name = "MousekinRace_Settlement_CapitalCity".Translate();

                // Only for "large" kingdoms
                if (kingdomSettlements.Count >= minNumSettlementsForFort) 
                {
                    // Ensures we don't pick a settlement that we already renamed as the capital city
                    List<Settlement> bigKingdomSettlements = kingdomSettlements.Where(x => x.Name != "MousekinRace_Settlement_CapitalCity".Translate()).ToList();

                    bigKingdomSettlements.RandomElement().Name = "MousekinRace_Settlement_Fort".Translate();
                }
            }
        }
    }
}
