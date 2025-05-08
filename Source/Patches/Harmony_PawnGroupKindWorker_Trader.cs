using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MousekinRace
{
    // Replace trader pawnkind in Mousekin Kingdom Slave Trader caravans with special Mousekin Slave Trader
    [HarmonyPatch(typeof(PawnGroupKindWorker_Trader), nameof(PawnGroupKindWorker_Trader.GenerateTrader))]
    public static class Harmony_PawnGroupKindWorker_Trader_GenerateTrader_OverridePawnkindForMousekinSlavers
    {
        // Use detour, as simply prefixing PawnGroupMaker groupMaker permanently overwrites the trader pawnkind for subsequent traders
        static bool Prefix(PawnGroupMakerParms parms, TraderKindDef traderKind, ref Pawn __result)
        {
            if (parms.faction.ideos.PrimaryCulture.IsMousekinKingdomLike() && traderKind.category == IncidentWorker_TraderCaravanArrival.SlaverTraderKindCategory)
            {
                Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(MousekinDefOf.MousekinTraderSlaver, parms.faction, PawnGenerationContext.NonPlayer, fixedIdeo: parms.ideo, tile: parms.tile, forceGenerateNewPawn: false, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, colonistRelationChanceFactor: 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: parms.inhabitants));
                pawn.mindState.wantsToTradeWithColony = true;
                PawnComponentsUtility.AddAndRemoveDynamicComponents(pawn, actAsIfSpawned: true);
                pawn.trader.traderKind = traderKind;
                parms.points -= pawn.kindDef.combatPower;
                __result = pawn;
                return false;
            }
            return true;
        }
    }

    // Curate guards for Mousekin trade caravans
    // - Reduce number of guards
    // - Replace all guards in Mousekin Kingdom Slave Trader caravans with special Mousekin Slave Trader pawnkinds
    [HarmonyPatch(typeof(PawnGroupKindWorker_Trader), nameof(PawnGroupKindWorker_Trader.GenerateGuards))]
    public static class Harmony_PawnGroupKindWorker_Trader_GenerateGuards_OverridePawnkindForMousekinSlavers
    {
        // Use detour, as simply prefixing PawnGroupMaker groupMaker permanently overwrites the trader pawnkind for subsequent traders
        static bool Prefix(PawnGroupMakerParms parms, PawnGroupMaker groupMaker, Pawn trader, ref List<Pawn> outPawns)
        {
            // Reduce number of guards (always run)
            if (parms.faction.ideos.PrimaryCulture.IsMousekin())
            {
                parms.points = Rand.Range(200, 250);
                if (trader.kindDef == MousekinDefOf.MousekinTraderSlaver)
                {
                    parms.points = Rand.Range(80, 120);
                }
            }

            if (parms.faction.ideos.PrimaryCulture.IsMousekinKingdomLike() && trader.kindDef == MousekinDefOf.MousekinTraderSlaver)
            {
                foreach (PawnGenOptionWithXenotype item2 in PawnGroupMakerUtility.ChoosePawnGenOptionsByPoints(parms.points, groupMaker.guards, parms))
                {
                    Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(MousekinDefOf.MousekinTraderSlaver, parms.faction, PawnGenerationContext.NonPlayer, fixedIdeo: parms.ideo, tile: parms.tile, forceGenerateNewPawn: false, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, colonistRelationChanceFactor: 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: parms.inhabitants));
                    outPawns.Add(pawn);
                }
                return false;
            }
            return true;
        }
    }
}
