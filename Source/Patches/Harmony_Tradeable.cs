using HarmonyLib;
using RimWorld;
using Verse;

namespace MousekinRace
{
    // Apply a trade price multiplier if the trader faction matches the player allegiance
    [HarmonyPatch(typeof(Tradeable), nameof(Tradeable.GetPriceFor))]
    public static class Harmony_Tradeable_GetPriceFor_UseMultiplierIfAllegianceSet
    {
        static void Postfix(ref Tradeable __instance, ref float __result)
        {
            /*Faction alignedFaction = GameComponent_Allegiance.Instance.alignedFaction;
            if (alignedFaction != null && alignedFaction == TradeSession.trader.Faction)
            {
                float tradePriceFactor = alignedFaction.def.GetModExtension<AlliableFactionExtension>().tradePriceFactor;
                __result *= tradePriceFactor;
            }*/
        }
    }

    // Insert a tooltip explanation for the trade price multiplier if the trader faction matches the player allegiance
    [HarmonyPatch(typeof(Tradeable), nameof(Tradeable.GetPriceTooltip))]
    public static class Harmony_Tradeable_GetPriceTooltip_AddTooltipIfAllegianceSet
    {
        static void Postfix(TradeAction action, ref string __result)
        {
            if (__result == null || action is not (TradeAction.PlayerBuys or TradeAction.PlayerSells))
            {
                return;
            } 

            /*Faction alignedFaction = GameComponent_Allegiance.Instance.alignedFaction;
            if (alignedFaction != null && alignedFaction == TradeSession.trader.Faction )
            {
                float tradePriceFactor = alignedFaction.def.GetModExtension<AlliableFactionExtension>().tradePriceFactor;
                if (tradePriceFactor != 1.00f)
                {
                    int finalBitIndex = __result.LastIndexOf("\n\n" + "YourNegotiatorBonus".Translate());
                    string finalBit = __result.Substring(finalBitIndex, __result.Length - finalBitIndex);
                    __result = __result.Replace(finalBit, "");
                    __result += "\n  x " + tradePriceFactor.ToString("F2") + " (" + "MousekinRace_TradePriceFactor_FactionAllegiance".Translate() + ")";
                    __result += finalBit;
                }
            }*/
        }
    }
}
