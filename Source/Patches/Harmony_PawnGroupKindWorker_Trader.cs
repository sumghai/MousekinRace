using HarmonyLib;
using RimWorld;
using Verse;

namespace MousekinRace
{
    // If a caravan trader is a slaver from the Mousekin Kingdom faction,
    // replace the trader with a special Mousekin Slave Trader pawnkind
    [HarmonyPatch(typeof(PawnGroupKindWorker_Trader), nameof(PawnGroupKindWorker_Trader.GenerateTrader))]
    public static class Harmony_PawnGroupKindWorker_Trader_GenerateTrader_OverridePawnkindForMousekinSlavers
    {
        static void Prefix(PawnGroupMakerParms parms, ref PawnGroupMaker groupMaker, TraderKindDef traderKind)
        {
            if (parms.faction.ideos.PrimaryCulture.IsMousekinKingdomLike() && traderKind.category == IncidentWorker_TraderCaravanArrival.SlaverTraderKindCategory)
            { 
                groupMaker.traders.Clear();
                groupMaker.traders.Add(new(){ 
                    kind = MousekinDefOf.MousekinTraderSlaver,
                    selectionWeight = 1,
                });

            }
        }
    }

    // If a caravan trader is a slaver from the Mousekin Kingdom faction (i.e. led by a Mousekin Slave Trader pawnkind),
    // replace all guards with special Mousekin Slave Trader pawnkinds
    [HarmonyPatch(typeof(PawnGroupKindWorker_Trader), nameof(PawnGroupKindWorker_Trader.GenerateGuards))]
    public static class Harmony_PawnGroupKindWorker_Trader_GenerateGuards_OverridePawnkindForMousekinSlavers
    {
        static void Prefix(PawnGroupMakerParms parms, ref PawnGroupMaker groupMaker, Pawn trader)
        {
            if (parms.faction.ideos.PrimaryCulture.IsMousekinKingdomLike() && trader.kindDef == MousekinDefOf.MousekinTraderSlaver)
            {
                groupMaker.guards.Clear();
                groupMaker.guards.Add(new()
                {
                    kind = MousekinDefOf.MousekinTraderSlaver,
                    selectionWeight = 1,
                });
            }
        }
    }

    // Reduce number of guards for Mousekin caravan traders
    [HarmonyPatch(typeof(PawnGroupKindWorker_Trader), nameof(PawnGroupKindWorker_Trader.GenerateGuards))]
    public static class Harmony_PawnGroupKindWorker_Trader_GenerateGuards_ReduceNumberForMousekinTraders
    {
        static void Prefix(ref PawnGroupMakerParms parms, Pawn trader)
        {
            if (parms.faction.ideos.PrimaryCulture.IsMousekin())
            {
                parms.points = Rand.Range(200, 250);
                if (trader.kindDef == MousekinDefOf.MousekinTraderSlaver)
                {
                    parms.points = Rand.Range(80, 120);
                }
            }
        }
    }
}
