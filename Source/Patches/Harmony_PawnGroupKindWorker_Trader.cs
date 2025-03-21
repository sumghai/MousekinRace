using HarmonyLib;
using RimWorld;

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
}
